using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.ClientOIDC
{
    public class TokenAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;

        public TokenAuthenticationStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public Task LogInAsync(string accessToken, string identityToken, DateTime expiry = default, IEnumerable<Claim> userInfo = default)
        {
            return SetAccessTokenAsync(true, accessToken, identityToken, expiry, userInfo);
        }

        public Task LogOutAsync()
        {
            return SetAccessTokenAsync(true, null);
        }

        public ValueTask SetPCKEVerifierAsync(string codeVerifier)
        {
            return _jsRuntime.SetItemAsync("code_verifier", codeVerifier);
        }

        public ValueTask<string> GetPCKEVerifierAsync()
        {
            return _jsRuntime.GetItemAsync<string>("code_verifier");
        }

        public ValueTask<string> GetAccessTokenAsync()
        {
            return _jsRuntime.GetItemAsync<string>("access_token");
        }

        public ValueTask<string> GetIdentityTokenAsync()
        {
            return _jsRuntime.GetItemAsync<string>("identity_token");
        }

        public async ValueTask<List<Claim>> GetUserInfoAsync()
        {
            var dict = await _jsRuntime.GetItemAsync<Dictionary<string, object>>("user_info");
            return dict != null ? dict.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())).ToList() : new List<Claim>();
        }

        private async Task SetAccessTokenAsync(bool refreshUI, string accessToken, string identityToken = null, DateTime expiry = default, IEnumerable<Claim> claims = default)
        {
            if (accessToken == null)
            {
                await _jsRuntime.RemoveItemAsync("access_token");
                await _jsRuntime.RemoveItemAsync("identity_token");
                await _jsRuntime.RemoveItemAsync("user_info");
                await _jsRuntime.RemoveItemAsync("expiry");
            }
            else
            {
                await _jsRuntime.SetItemAsync("access_token", accessToken);
                await _jsRuntime.SetItemAsync("identity_token", identityToken);
                await _jsRuntime.SetItemAsync("user_info", claims != null ? claims.ToDictionary(c => c.Type, c => c.Value) : null);
                await _jsRuntime.SetItemAsync("expiry", expiry);
            }

            if(refreshUI)
            {
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            }
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //OAuth = authorization = access_token + id_token
            //OpenId = authentication = id_token

            //Read token and validate

            //https://github.com/dotnet/aspnetcore/blob/87a92e52c8b4bb7cb75ff78d53d641b1d34f8775/src/Security/Authentication/JwtBearer/src/JwtBearerHandler.cs
            //https://github.com/dotnet/aspnetcore/blob/8b000d961cd3ccfcc8090fb8368fd6598bace978/src/Security/Authentication/OpenIdConnect/src/OpenIdConnectHandler.cs
            //TokenValidationParameters validationParameters =
            //new TokenValidationParameters
            //{
            //    ValidateIssuer = true,
            //    ValidIssuer = auth0Domain
            //    ValidateAudience = true,
            //    ValidAudiences = new[] { auth0Audience }
            //    ValidateIssuerSigningKey = false,
            //    RequireSignedTokens = false,
            //    ValidateLifetime = true,
            //    RequireExpirationTime = false
            //};

            //https://www.mikesdotnetting.com/article/342/managing-authentication-token-expiry-in-webassembly-based-blazor
            //SecurityToken validatedToken;
            //JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler(); //Used in OpenIdConnectOptions and JwtBearerOptions
            //var user = handler.ValidateToken("eyJhbGciOi.....", validationParameters, out validatedToken);
            //var claimsIdentity = (ClaimsIdentity)user.Identity;

            var accessToken = await GetTokenAsync();
            var userInfo = await GetUserInfoAsync();
            var identity = string.IsNullOrEmpty(accessToken)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(ParseClaimsFromJwt(accessToken, userInfo), "Jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        private async Task<string> GetTokenAsync()
        {
            var expiry = await _jsRuntime.GetItemAsync<DateTime>("expiry");
            if (expiry > DateTime.Now)
            {
                return await _jsRuntime.GetItemAsync<string>("access_token");
            }
            else
            {
                await SetAccessTokenAsync(false, null);
                return null;
            }
        }

        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt, IEnumerable<Claim> userInfo)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            if (userInfo != null)
                claims.AddRange(userInfo);

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
