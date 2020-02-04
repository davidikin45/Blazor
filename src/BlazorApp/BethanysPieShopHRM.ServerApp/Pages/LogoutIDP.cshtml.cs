using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BethanysPieShopHRM.Server.Pages
{
    public class LogoutIDPModel : PageModel
    {
        public async Task OnGetAsync()
        {
            //Actually what happens is HttpContext.SignOutAsync("oidc"); sets a default ActionResult(Which is to redirect to the OpenIdConnect provider to complete the sign -out
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}