using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.ClientOIDC.Pages
{
    public class SignOutOIDCBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public TokenAuthenticationStateProvider TokenAuthenticationStateProvider { get; set; }


        protected override Task OnInitializedAsync()
        {
            NavigationManager.NavigateTo("");
            return Task.CompletedTask;
        }
    }
}