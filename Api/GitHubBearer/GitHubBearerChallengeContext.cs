using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surbowl.AspNetCore.Authentication.GitHubBearer
{
	public class GitHubBearerChallengeContext : PropertiesContext<GitHubBearerOptions>
	{
		public Exception AuthenticateFailure
		{
			get;
			set;
		}

		public string Error
		{
			get;
			set;
		}

		public string ErrorDescription
		{
			get;
			set;
		}

		public string ErrorUri
		{
			get;
			set;
		}

		public bool Handled
		{
			get;
			private set;
		}

		public GitHubBearerChallengeContext(HttpContext context, AuthenticationScheme scheme, GitHubBearerOptions options, AuthenticationProperties properties)
			: base(context, scheme, options, properties)
		{
		}

		public void HandleResponse()
		{
			Handled = true;
		}
	}

}
