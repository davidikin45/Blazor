using BethanysPieShopHRM.ClientOIDC.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.ClientOIDC.Pages
{
    public class LoginIDPBase : ComponentBase
    {
        [CascadingParameter]
        Task<AuthenticationState> authenticationStateTask { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public TokenAuthenticationStateProvider TokenAuthenticationStateProvider { get; set; }

        [Inject]
        public IOpenIDConnectService OpenIDConnectService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            var authState = await authenticationStateTask;
            if (!authState.User.Identity.IsAuthenticated)
            {
                var resp = await OpenIDConnectService.CreateAuthorizeUrlAsync();
                await TokenAuthenticationStateProvider.SetPCKEVerifierAsync(resp.CodeVerifier);
                NavigationManager.NavigateTo(resp.AuthorizeUrl);
            }
            else
            {
                // redirect to the root
                NavigationManager.NavigateTo("");
            }
        }
    }
}