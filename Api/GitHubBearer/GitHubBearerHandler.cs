using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Surbowl.AspNetCore.Authentication.GitHubBearer
{
	public class GitHubBearerHandler : AuthenticationHandler<GitHubBearerOptions>
	{
		private OpenIdConnectConfiguration _configuration;

		protected GitHubBearerEvents Events
		{
			get
			{
				return (GitHubBearerEvents)base.Events;
			}
			set
			{
				base.Events=value;
			}
		}

		public GitHubBearerHandler(IOptionsMonitor<GitHubBearerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
			: base(options, logger, encoder, clock)
		{
		}

		protected override Task<object> CreateEventsAsync()
		{
			return Task.FromResult((object)new GitHubBearerEvents());
		}

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			AuthenticateResult result = default(AuthenticateResult);
			try
			{
				MessageReceivedContext messageReceivedContext = new MessageReceivedContext(base.Context, base.Scheme, base.Options);
				await Events.MessageReceived(messageReceivedContext);
				if (messageReceivedContext.Result == null)
				{
					string token = messageReceivedContext.Token;
					if (string.IsNullOrEmpty(token))
					{
						string text = base.get_Request().Headers.get_Item(HeaderNames.Authorization);
						if (string.IsNullOrEmpty(text))
						{
							result = AuthenticateResult.NoResult();
							return result;
						}
						if (text.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
						{
							token = text.Substring("Bearer ".Length).Trim();
						}
						if (string.IsNullOrEmpty(token))
						{
							result = AuthenticateResult.NoResult();
							return result;
						}
					}
					if (_configuration == null && base.Options.ConfigurationManager != null)
					{
						_configuration = await base.Options.ConfigurationManager.GetConfigurationAsync(base.Context.get_RequestAborted());
					}
					TokenValidationParameters tokenValidationParameters = base.Options.TokenValidationParameters.Clone();
					if (_configuration != null)
					{
						string[] obj = new string[1]
						{
						_configuration.Issuer
						};
						tokenValidationParameters.ValidIssuers = (tokenValidationParameters.ValidIssuers?.Concat(obj) ?? obj);
						tokenValidationParameters.IssuerSigningKeys = (tokenValidationParameters.IssuerSigningKeys?.Concat(_configuration.SigningKeys) ?? _configuration.SigningKeys);
					}
					List<Exception> list = null;
					foreach (ISecurityTokenValidator securityTokenValidator in base.Options.SecurityTokenValidators)
					{
						if (securityTokenValidator.CanReadToken(token))
						{
							ClaimsPrincipal principal;
							SecurityToken validatedToken;
							try
							{
								principal = securityTokenValidator.ValidateToken(token, tokenValidationParameters, out validatedToken);
							}
							catch (Exception ex)
							{
								base.Logger.TokenValidationFailed(ex);
								if (base.Options.RefreshOnIssuerKeyNotFound && base.Options.ConfigurationManager != null && ex is SecurityTokenSignatureKeyNotFoundException)
								{
									base.Options.ConfigurationManager.RequestRefresh();
								}
								if (list == null)
								{
									list = new List<Exception>(1);
								}
								list.Add(ex);
								continue;
							}
							base.Logger.TokenValidationSucceeded();
							TokenValidatedContext tokenValidatedContext2 = new TokenValidatedContext(base.Context, base.Scheme, base.Options);
							tokenValidatedContext2.set_Principal(principal);
							tokenValidatedContext2.SecurityToken = validatedToken;
							TokenValidatedContext tokenValidatedContext = tokenValidatedContext2;
							await Events.TokenValidated(tokenValidatedContext);
							if (tokenValidatedContext.Result == null)
							{
								if (base.Options.SaveToken)
								{
									AuthenticationProperties properties = tokenValidatedContext.get_Properties();
									AuthenticationToken[] obj2 = new AuthenticationToken[1];
									AuthenticationToken val = new AuthenticationToken();
									val.set_Name("access_token");
									val.set_Value(token);
									obj2[0] = val;
									AuthenticationTokenExtensions.StoreTokens(properties, (IEnumerable<AuthenticationToken>)obj2);
								}
								tokenValidatedContext.Success();
								result = tokenValidatedContext.Result;
								return result;
							}
							result = tokenValidatedContext.Result;
							return result;
						}
					}
					if (list == null)
					{
						result = AuthenticateResult.Fail("No SecurityTokenValidator available for token: " + token);
						return result;
					}
					AuthenticationFailedContext authenticationFailedContext2 = new AuthenticationFailedContext(base.Context, base.Scheme, base.Options)
					{
						Exception = ((list.Count == 1) ? list[0] : new AggregateException(list))
					};
					await Events.AuthenticationFailed(authenticationFailedContext2);
					if (authenticationFailedContext2.Result == null)
					{
						result = AuthenticateResult.Fail(authenticationFailedContext2.Exception);
						return result;
					}
					result = authenticationFailedContext2.Result;
					return result;
				}
				result = messageReceivedContext.Result;
				return result;
			}
			catch (Exception ex2)
			{
				// base.Logger.ErrorProcessingMessage(ex2);
				AuthenticationFailedContext authenticationFailedContext2 = new AuthenticationFailedContext(base.Context, base.Scheme, base.Options)
				{
					Exception = ex2
				};
				await Events.AuthenticationFailed(authenticationFailedContext2);
				if (authenticationFailedContext2.Result != null)
				{
					return authenticationFailedContext2.Result;
				}
				Exception obj3 = ex2 as Exception;
				if (obj3 == null)
				{
					throw ex2;
				}
				ExceptionDispatchInfo.Capture(obj3).Throw();
			}
			return result;
		}

		protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
		{
			AuthenticateResult val = await base.HandleAuthenticateOnceSafeAsync();
			GitHubBearerChallengeContext eventContext = new GitHubBearerChallengeContext(base.Context, base.Scheme, base.Options, properties)
			{
				AuthenticateFailure = ((val != null) ? val.Failure : null)
			};
			if (base.Options.IncludeErrorDetails && eventContext.AuthenticateFailure != null)
			{
				eventContext.Error = "invalid_token";
				eventContext.ErrorDescription = CreateErrorDescription(eventContext.AuthenticateFailure);
			}
			await Events.Challenge(eventContext);
			if (!eventContext.Handled)
			{
				base.Response.StatusCode = 401;
				if (string.IsNullOrEmpty(eventContext.Error) && string.IsNullOrEmpty(eventContext.ErrorDescription) && string.IsNullOrEmpty(eventContext.ErrorUri))
				{
					HeaderDictionaryExtensions.Append(base.Response.Headers, HeaderNames.WWWAuthenticate, (StringValues)base.Options.Challenge);
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder(base.Options.Challenge);
					if (base.Options.Challenge.IndexOf(' ') > 0)
					{
						stringBuilder.Append(',');
					}
					if (!string.IsNullOrEmpty(eventContext.Error))
					{
						stringBuilder.Append(" error=\"");
						stringBuilder.Append(eventContext.Error);
						stringBuilder.Append("\"");
					}
					if (!string.IsNullOrEmpty(eventContext.ErrorDescription))
					{
						if (!string.IsNullOrEmpty(eventContext.Error))
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append(" error_description=\"");
						stringBuilder.Append(eventContext.ErrorDescription);
						stringBuilder.Append('"');
					}
					if (!string.IsNullOrEmpty(eventContext.ErrorUri))
					{
						if (!string.IsNullOrEmpty(eventContext.Error) || !string.IsNullOrEmpty(eventContext.ErrorDescription))
						{
							stringBuilder.Append(",");
						}
						stringBuilder.Append(" error_uri=\"");
						stringBuilder.Append(eventContext.ErrorUri);
						stringBuilder.Append('"');
					}
					HeaderDictionaryExtensions.Append(base.Response.Headers, HeaderNames.WWWAuthenticate, (StringValues)stringBuilder.ToString());
				}
			}
		}

		protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
		{
			ForbiddenContext context = new ForbiddenContext(base.Context, base.Scheme, base.Options);
			base.Response.StatusCode = 403;
			return Events.Forbidden(context);
		}

		private static string CreateErrorDescription(Exception authFailure)
		{
			AggregateException ex = authFailure as AggregateException;
			IEnumerable<Exception> enumerable = (IEnumerable<Exception>)((ex == null) ? ((object)new Exception[1]
			{
			authFailure
			}) : ((object)ex.InnerExceptions));
			List<string> list = new List<string>();
			foreach (Exception item in enumerable)
			{
				SecurityTokenInvalidAudienceException ex2 = item as SecurityTokenInvalidAudienceException;
				if (ex2 == null)
				{
					SecurityTokenInvalidIssuerException ex3 = item as SecurityTokenInvalidIssuerException;
					if (ex3 == null)
					{
						if (!(item is SecurityTokenNoExpirationException))
						{
							SecurityTokenInvalidLifetimeException ex4 = item as SecurityTokenInvalidLifetimeException;
							if (ex4 == null)
							{
								SecurityTokenNotYetValidException ex5 = item as SecurityTokenNotYetValidException;
								if (ex5 == null)
								{
									SecurityTokenExpiredException ex6 = item as SecurityTokenExpiredException;
									if (ex6 == null)
									{
										if (!(item is SecurityTokenSignatureKeyNotFoundException))
										{
											if (item is SecurityTokenInvalidSignatureException)
											{
												list.Add("The signature is invalid");
											}
										}
										else
										{
											list.Add("The signature key was not found");
										}
									}
									else
									{
										list.Add("The token expired at '" + ex6.Expires.ToString(CultureInfo.InvariantCulture) + "'");
									}
								}
								else
								{
									list.Add("The token is not valid before '" + ex5.NotBefore.ToString(CultureInfo.InvariantCulture) + "'");
								}
							}
							else
							{
								list.Add("The token lifetime is invalid; NotBefore: '" + (ex4.NotBefore?.ToString(CultureInfo.InvariantCulture) ?? "(null)") + "', Expires: '" + (ex4.Expires?.ToString(CultureInfo.InvariantCulture) ?? "(null)") + "'");
							}
						}
						else
						{
							list.Add("The token has no expiration");
						}
					}
					else
					{
						list.Add("The issuer '" + (ex3.InvalidIssuer ?? "(null)") + "' is invalid");
					}
				}
				else
				{
					list.Add("The audience '" + (ex2.InvalidAudience ?? "(null)") + "' is invalid");
				}
			}
			return string.Join("; ", list);
		}
	}

}
