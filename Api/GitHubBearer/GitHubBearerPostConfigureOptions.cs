using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Surbowl.AspNetCore.Authentication.GitHubBearer
{
	public class GitHubBearerPostConfigureOptions : IPostConfigureOptions<GitHubBearerOptions>
	{
		public void PostConfigure(string name, GitHubBearerOptions options)
		{
			if (string.IsNullOrEmpty(options.TokenValidationParameters.ValidAudience) && !string.IsNullOrEmpty(options.Audience))
			{
				options.TokenValidationParameters.ValidAudience = options.Audience;
			}
			if (options.ConfigurationManager == null)
			{
				if (options.Configuration != null)
				{
					options.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(options.Configuration);
				}
				else if (!string.IsNullOrEmpty(options.MetadataAddress) || !string.IsNullOrEmpty(options.Authority))
				{
					if (string.IsNullOrEmpty(options.MetadataAddress) && !string.IsNullOrEmpty(options.Authority))
					{
						options.MetadataAddress = options.Authority;
						if (!options.MetadataAddress.EndsWith("/", StringComparison.Ordinal))
						{
							options.MetadataAddress += "/";
						}
						options.MetadataAddress += ".well-known/openid-configuration";
					}
					if (options.RequireHttpsMetadata && !options.MetadataAddress.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
					{
						throw new InvalidOperationException("The MetadataAddress or Authority must use HTTPS unless disabled for development by setting RequireHttpsMetadata=false.");
					}
					HttpClient httpClient = new HttpClient(options.BackchannelHttpHandler ?? new HttpClientHandler());
					httpClient.Timeout = options.BackchannelTimeout;
					httpClient.MaxResponseContentBufferSize = 10485760L;
					options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(options.MetadataAddress, new OpenIdConnectConfigurationRetriever(), new HttpDocumentRetriever(httpClient)
					{
						RequireHttps = options.RequireHttpsMetadata
					});
				}
			}
		}
	}

}
