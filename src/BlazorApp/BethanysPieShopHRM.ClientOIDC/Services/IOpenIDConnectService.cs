using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.ClientOIDC.Services
{
    public interface IOpenIDConnectService
    {
        //Client Credentials
        Task<(string AccessToken, DateTime Expiry)> GetApiAccessTokenAsync();

        //Authorization Code Flow (PKCE)
        Task<(string AuthorizeUrl, string CodeVerifier)> CreateAuthorizeUrlAsync();
        Task<string> CreateLogoutUrlAsync(string idTokenHint);
        Task<(string AccessToken, string IdentityToken, DateTime Expiry)> GetApiAccessTokenAsync(string code, string codeVerifier);

        Task<IEnumerable<Claim>> GetUserInfoAsync(string accessToken);

        Task RevokeAccessAsync(string accessToken, string refreshToken = null);

        Task<(string AccessToken, DateTime Expiry, string RefreshToken, string IdentityToken)> RenewTokensAsync(string refreshToken);
    }
}
