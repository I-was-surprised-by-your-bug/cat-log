using System;
using System.Collections.Generic;
using System.Text;

namespace Surbowl.AspNetCore.Authentication.GitHubBearer
{
    class GitHubBearerDefaults
    {
        /// <summary>
        /// Default value for <see cref="AuthenticationScheme.Name"/>.
        /// </summary>
        public const string AuthenticationScheme = "GitHubBearer";

        /// <summary>
        /// Default value for <see cref="AuthenticationScheme.DisplayName"/>.
        /// </summary>
        public const string DisplayName = "GitHubBearer";

        /// <summary>
        /// Default value for UserInformationEndpoint.
        /// </summary>
        public const string UserInformationEndpoint = "https://api.github.com/user";
    }
}
