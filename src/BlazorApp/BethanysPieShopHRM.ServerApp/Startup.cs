using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using BethanysPieShopHRM.Server;
using BethanysPieShopHRM.Server.Interceptors;
using BethanysPieShopHRM.Server.Services;
using Blazor.IndexedDB.Framework;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
            //Blazor.IndexedDB.Framework
            //services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
            //services.AddSingleton<AppIndexedDb>(sp => sp.GetRequiredService<IIndexedDbFactory>().Create<AppIndexedDb>().Result);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: "db");
            });

            services.AddRazorPages();
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
            //Using Microsoft.Azure.SignalR with config Azure:SignalR:ConnectionString
            //services.AddSignalR().AddAzureSignalR("ConnectionString"); // .Net Core 2.x only\

            services.AddHttpContextAccessor();

            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(); //Login
            //services.AddAuthentication(IdentityConstants.ApplicationScheme).AddCookie(); //ASP.NET Identity Login
            
            //OpenIdConnect
            //https://www.scottbrady91.com/OpenID-Connect/ASPNET-Core-using-Proof-Key-for-Code-Exchange-PKCE
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
                options.ExpireTimeSpan = TimeSpan.FromDays(14); //When persist is used.
                options.SlidingExpiration = true;
            })
               .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,
                options =>
                {
                    options.Authority = "https://localhost:44333";
                    options.ClientId = "bethanyspieshophr";
                    options.ClientSecret = "108B7B4F-BEFC-4DD2-82E1-7F025F0F75D0";
                    options.ResponseType = "code id_token"; //hybrid
                    options.Scope.Add("openid");//access_token
                    options.Scope.Add("profile");//access_token
                    options.Scope.Add("email");//access_token
                    options.Scope.Add("bethanyspieshophrapi");//access_token
                    options.Scope.Add("country");//access_token
                    options.ClaimActions.MapUniqueJsonKey("country", "country");//id_token > User
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
            services.AddTransient<BearerHttpHandler>();
            services.AddTransient<AuthorizationProxyHttpHandler>();

            /// --- Blazor Server > API --- ///
            services.AddHttpClient("api", client =>
            {
                client.BaseAddress = new Uri("https://localhost:44340/");
            })
           .AddHttpMessageHandler<BearerHttpHandler>()
           .AddHttpMessageHandler<BlazorDisplaySpinnerAutomaticallyHttpMessageHandler>()
           .ConfigurePrimaryHttpMessageHandler(handler => new HttpClientHandler()
           {
               AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.Brotli
           });

            //services.AddScoped<IEmployeeDataService, MockEmployeeDataService>();
            services.AddScoped<IEmployeeDataService>(sp => new EmployeeDataService(sp.GetService<IHttpClientFactory>().CreateClient("api")));
            services.AddScoped<ICountryDataService>(sp => new CountryDataService(sp.GetService<IHttpClientFactory>().CreateClient("api")));
            services.AddScoped<IJobCategoryDataService>(sp => new JobCategoryDataService(sp.GetService<IHttpClientFactory>().CreateClient("api")));
            services.AddScoped<IBenefitDataService>(sp => new BenefitDataService(sp.GetService<IHttpClientFactory>().CreateClient("api")));

            //Spinner
            services.AddTransient<BlazorDisplaySpinnerAutomaticallyHttpMessageHandler>();
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
