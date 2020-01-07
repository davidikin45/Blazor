using System;
using Marvin.IDP.Areas.Identity.Data;
using Marvin.IDP.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Marvin.IDP.Areas.Identity.IdentityHostingStartup))]
namespace Marvin.IDP.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<IdentityContext>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("IdentityContextConnection")));

                //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                //    .AddEntityFrameworkStores<IdentityContext>();

                services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();//Account Activation or Change Password Tokens

                services.AddTransient<IEmailSender, DummyEmailSender>();
            });
        }
    }
}