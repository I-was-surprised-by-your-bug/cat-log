using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surbowl.AspNetCore.Authentication.GitHubBearer
{
	public class ForbiddenContext : ResultContext<GitHubBearerOptions>
	{
		public ForbiddenContext(HttpContext context, AuthenticationScheme scheme, GitHubBearerOptions options)
								: base(context, scheme, options)
		{
		}
	}
}
