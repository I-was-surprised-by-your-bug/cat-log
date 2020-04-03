using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surbowl.AspNetCore.Authentication.GitHubBearer
{
	public class TokenValidatedContext : ResultContext<GitHubBearerOptions>
	{
		public SecurityToken SecurityToken
		{
			get;
			set;
		}

		public TokenValidatedContext(HttpContext context, AuthenticationScheme scheme, GitHubBearerOptions options)
			: base(context, scheme, options)
		{
		}
	}

}
