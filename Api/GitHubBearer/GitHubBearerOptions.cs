using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace Surbowl.AspNetCore.Authentication.GitHubBearer
{
    public class GitHubBearerOptions : AuthenticationSchemeOptions
    {
        public bool RequireHttpsMetadata
        {
            get;
            set;
        } = true;


        public string MetadataAddress
        {
            get;
            set;
        }

        public string Authority
        {
            get;
            set;
        }

        public string Audience
        {
            get;
            set;
        }

        public string Challenge
        {
            get;
            set;
        } = "GitHubBearer";


        public GitHubBearerEvents Events
        {
            get
            {
                return (GitHubBearerEvents)this.Events;
            }
            set
            {
                this.Events = value;
            }
        }

        public HttpMessageHandler BackchannelHttpHandler
        {
            get;
            set;
        }

        public TimeSpan BackchannelTimeout
        {
            get;
            set;
        } = TimeSpan.FromMinutes(1.0);


        public OpenIdConnectConfiguration Configuration
        {
            get;
            set;
        }

        public IConfigurationManager<OpenIdConnectConfiguration> ConfigurationManager
        {
            get;
            set;
        }

        public bool RefreshOnIssuerKeyNotFound
        {
            get;
            set;
        } = true;


        public IList<ISecurityTokenValidator> SecurityTokenValidators
        {
            get;
        } = new List<ISecurityTokenValidator>
    {
        new JwtSecurityTokenHandler()
    };


        public TokenValidationParameters TokenValidationParameters
        {
            get;
            set;
        } = new TokenValidationParameters();


        public bool SaveToken
        {
            get;
            set;
        } = true;


        public bool IncludeErrorDetails
        {
            get;
            set;
        } = true;


        public GitHubBearerOptions()
            //: this()
        {
        }
    }
}