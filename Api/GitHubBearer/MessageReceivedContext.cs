using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surbowl.AspNetCore.Authentication.GitHubBearer
{
	public class MessageReceivedContext : ResultContext<GitHubBearerOptions>
	{
		public string Token
		{
			get;
			set;
		}

		public MessageReceivedContext(HttpContext context, AuthenticationScheme scheme, GitHubBearerOptions options)
			: base(context, scheme, options)
		{
		}
	}
}
