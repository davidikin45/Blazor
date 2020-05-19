// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Marvin.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("roles", "Your roles(s)",new List<string>(){"role"} ),
                new IdentityResource("country",  "The country you're living in", new [] { "country" })
            };


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("bethanyspieshophrapi", 
                    "Bethany's Pie Shop HR API", 
                    new [] { "country" }) //include in access_token
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            { 
                 //Client Initiation to Server Api
                //ASP.NET Core 2 Authentication Playbook Video 8
                new Client
                {
                    ClientId = "bethanyspieshophr_spa",
                    ClientName = "Bethany's Pie Shop HR SPA",
                    AllowedGrantTypes = GrantTypes.Code, // or Implicit
                    RequirePkce = true,
                    RequireClientSecret = false,
                    AccessTokenType = AccessTokenType.Jwt,
                    AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, IdentityServerConstants.StandardScopes.Email, "roles", "country" , "bethanyspieshophrapi" },
                    RedirectUris = { "http://localhost:53779/signin-oidc" , "http://localhost:53779/authentication/login-callback" },
                    PostLogoutRedirectUris = { "http://localhost:53779/signout-callback-oidc", "http://localhost:53779/authentication/signout-callback" },
                    AllowedCorsOrigins = { "http://localhost:53779" }, //CORS for IDP
                    RequireConsent = false, //Consent screen,
                    AllowOfflineAccess = false, //Refresh Tokens
                    AccessTokenLifetime = 1200,
                },
                new Client
                {
                    ClientId = "bethanyspieshophr",
                    ClientName = "Bethany's Pie Shop HR",
                    AllowedGrantTypes = GrantTypes.Hybrid, 
                    ClientSecrets = { new Secret("108B7B4F-BEFC-4DD2-82E1-7F025F0F75D0".Sha256()) },
                    RedirectUris = { "https://localhost:44346/signin-oidc" }, 
                    PostLogoutRedirectUris = { "https://localhost:44346/signout-callback-oidc" },
                    AllowOfflineAccess = true, //Refresh Tokens
                    RequireConsent = false, //Consent screen not shown
                    AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, IdentityServerConstants.StandardScopes.Email, "roles", "country" , "bethanyspieshophrapi" } 
                }                 
            };
    }
}