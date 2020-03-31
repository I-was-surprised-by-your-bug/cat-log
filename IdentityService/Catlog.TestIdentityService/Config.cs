// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Catlog.TestIdentityService
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("catlog.api", "Catlog API")
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "catlog.client",
                    ClientName = "Catlog Client",

                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequirePkce = true,
                    ClientSecrets = { new Secret("69C1A7E1-AC79-4A89-A308-C46998FB86B2".Sha256()) },

                    RedirectUris = { "http://localhost:5000/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:5000/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:5000/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "catlog.api",
                                      IdentityServerConstants.StandardScopes.OpenId,
                                      IdentityServerConstants.StandardScopes.Profile}
                }
            };
    }
}