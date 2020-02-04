using BethanysPieShopHRM.ClientOIDC.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.ClientOIDC.Pages
{
    public class LogoutIDPBase : ComponentBase
    {
        [Inject]
        public TokenAuthenticationStateProvider TokenAuthenticationStateProvider { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IOpenIDConnectService OpenIDConnectService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await TokenAuthenticationStateProvider.LogOutAsync();
            var url = await OpenIDConnectService.CreateLogoutUrlAsync(await TokenAuthenticationStateProvider.GetIdentityTokenAsync());
            NavigationManager.NavigateTo(url);
        }
    }
}