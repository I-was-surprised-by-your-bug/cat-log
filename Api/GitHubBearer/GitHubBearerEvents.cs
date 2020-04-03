using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Surbowl.AspNetCore.Authentication.GitHubBearer
{
    public class GitHubBearerEvents
    {
        public Func<AuthenticationFailedContext, Task> OnAuthenticationFailed { get; set; } = (AuthenticationFailedContext context) => Task.CompletedTask;


        public Func<ForbiddenContext, Task> OnForbidden { get; set; } = (ForbiddenContext context) => Task.CompletedTask;


        public Func<MessageReceivedContext, Task> OnMessageReceived { get; set; } = (MessageReceivedContext context) => Task.CompletedTask;


        public Func<TokenValidatedContext, Task> OnTokenValidated { get; set; } = (TokenValidatedContext context) => Task.CompletedTask;


        public Func<GitHubBearerChallengeContext, Task> OnChallenge { get; set; } = (GitHubBearerChallengeContext context) => Task.CompletedTask;


        public virtual Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            return OnAuthenticationFailed(context);
        }

        public virtual Task Forbidden(ForbiddenContext context)
        {
            return OnForbidden(context);
        }

        public virtual Task MessageReceived(MessageReceivedContext context)
        {
            return OnMessageReceived(context);
        }

        public virtual Task TokenValidated(TokenValidatedContext context)
        {
            return OnTokenValidated(context);
        }

        public virtual Task Challenge(GitHubBearerChallengeContext context)
        {
            return OnChallenge(context);
        }
    }
}
