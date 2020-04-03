using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surbowl.AspNetCore.Authentication.GitHubBearer
{
	public class AuthenticationFailedContext : ResultContext<GitHubBearerOptions>
	{
		public Exception Exception
		{
			get;
			set;
		}

		public AuthenticationFailedContext(HttpContext context, AuthenticationScheme scheme, GitHubBearerOptions options)
										   : base(context, scheme, options)
		{
		}
	}
}
