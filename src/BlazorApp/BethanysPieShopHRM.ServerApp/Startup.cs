using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using BethanysPieShopHRM.Server.Interceptors;
using BethanysPieShopHRM.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BethanysPieShopHRM.ServerApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
            //Using Microsoft.Azure.SignalR with config Azure:SignalR:ConnectionString
            //services.AddSignalR().AddAzureSignalR("ConnectionString"); // .Net Core 2.x only\

            services.AddHttpContextAccessor();

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(); //Login
            //services.AddAuthentication(IdentityConstants.ApplicationScheme).AddCookie(); //ASP.NET Identity Login

            //OpenIdConnect
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,
                options =>
                {
                    options.Authority = "https://localhost:44333";
                    options.ClientId = "bethanyspieshophr";
                    options.ClientSecret = "108B7B4F-BEFC-4DD2-82E1-7F025F0F75D0";
                    options.ResponseType = "code id_token"; //hybrid
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("bethanyspieshophrapi");
                    options.Scope.Add("country");
                    options.ClaimActions.MapUniqueJsonKey("country", "country");
                    //options.CallbackPath = ...
                    options.SaveTokens = true; 
                    options.GetClaimsFromUserInfoEndpoint = true;

                });

            //Policies stored in shared assembly so can be accessed by Blazor and API
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    BethanysPieShopHRM.Shared.Policies.CanManageEmployees,
                    BethanysPieShopHRM.Shared.Policies.CanManageEmployeesPolicy());
            });

            //Blazor > Server > API
            services.AddTransient<AuthorizationJwtProxyHttpHandler>();

            //services.AddScoped<IEmployeeDataService, MockEmployeeDataService>();
            services.AddHttpClient<IEmployeeDataService, EmployeeDataService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:44340/");
            })
            .AddHttpMessageHandler<AuthorizationJwtProxyHttpHandler>()
            .ConfigurePrimaryHttpMessageHandler(handler => new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            });

            services.AddHttpClient<ICountryDataService, CountryDataService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:44340/");
            })
            .AddHttpMessageHandler<AuthorizationJwtProxyHttpHandler>()
            .ConfigurePrimaryHttpMessageHandler(handler => new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            });

            services.AddHttpClient<IJobCategoryDataService, JobCategoryDataService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:44340/");
            })
            .AddHttpMessageHandler<AuthorizationJwtProxyHttpHandler>()
            .ConfigurePrimaryHttpMessageHandler(handler => new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            });

            //Spinner
            services.AddScoped<ISpinnerService, SpinnerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
