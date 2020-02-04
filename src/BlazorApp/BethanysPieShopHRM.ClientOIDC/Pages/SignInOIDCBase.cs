using BethanysPieShopHRM.ClientOIDC.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.ClientOIDC.Pages
{
    public class SignInOIDCBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public TokenAuthenticationStateProvider TokenAuthenticationStateProvider { get; set; }

        [Inject]
        public IOpenIDConnectService OpenIDConnectService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);

            var code = "";
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out var value))
            {
                code = value.First();
            }

            var codeVerifier = await TokenAuthenticationStateProvider.GetPCKEVerifierAsync();

            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(codeVerifier))
            {
                try
                {
                    var result = await OpenIDConnectService.GetApiAccessTokenAsync(code, codeVerifier);
                    var claims = await OpenIDConnectService.GetUserInfoAsync(result.AccessToken);
                    await TokenAuthenticationStateProvider.LogInAsync(result.AccessToken, result.IdentityToken, result.Expiry, claims);
                }
                catch
                {

                }
            }

            // redirect to the root
            NavigationManager.NavigateTo("");
        }
    }
}