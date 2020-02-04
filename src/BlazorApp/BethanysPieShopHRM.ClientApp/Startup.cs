using BethanysPieShopHRM.ClientOIDC;
using BethanysPieShopHRM.ClientOIDC.Services;
using BethanysPieShopHRM.Server;
using BethanysPieShopHRM.Server.Interceptors;
using BethanysPieShopHRM.Server.Services;
using Blazor.IndexedDB.Framework;
using EntityFrameworkCore.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace BethanysPieShopHRM.ClientApp
{
    public class Startup
    {
        private Type MonoWasmHttpMessageHandlerType = Assembly.Load("WebAssembly.Net.Http").GetType("WebAssembly.Net.Http.HttpClient.WasmHttpMessageHandler");

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureHttpHandlers(services);
            ConfigureData(services);
            ConfigureDefaultHttpClient(services);
            ConfigureApi(services);
            ConfigureAuthorization(services);
        }

        public void ConfigureData(IServiceCollection services)
        {
            //Blazor.IndexedDB.Framework
            services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
            services.AddSingleton<AppIndexedDb>(sp => sp.GetRequiredService<IIndexedDbFactory>().Create<AppIndexedDb>().Result);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseLocalStorageDatabase(services.GetJSRuntime(), databaseName: "db");
            });

            //services.AddDbContext<AppDbContext>(options =>
            //{
            //    options.UseInMemoryDatabase(databaseName: "db").ForBlazorWebAssembly();
            //});
        }

        public void ConfigureHttpHandlers(IServiceCollection services)
        {
            services.AddScoped<BlazorDisplaySpinnerAutomaticallyHttpMessageHandler>();
            services.AddScoped(MonoWasmHttpMessageHandlerType);

            services.AddScoped<ISpinnerService, SpinnerService>();
        }

        public void ConfigureDefaultHttpClient(IServiceCollection services)
        {
            services.Remove(services.Single(x => x.ServiceType == typeof(HttpClient)));

            /// --- Blazor WASM > API --- ///
            //services.AddScoped<HttpClient>(s =>
            //{
            //    Type MonoWasmHttpMessageHandlerType = Assembly.Load("WebAssembly.Net.Http").GetType("WebAssembly.Net.Http.HttpClient.WasmHttpMessageHandler");
            //    var blazorDisplaySpinnerAutomaticallyHttpMessageHandler = s.GetRequiredService<BlazorDisplaySpinnerAutomaticallyHttpMessageHandler>();
            //    blazorDisplaySpinnerAutomaticallyHttpMessageHandler.InnerHandler = (HttpMessageHandler)s.GetService(MonoWasmHttpMessageHandlerType);

            //    var client = new HttpClient(blazorDisplaySpinnerAutomaticallyHttpMessageHandler)
            //    { BaseAddress = new System.Uri("https://localhost:44340/") };
            //    return client;
            //});

            /// --- Blazor WASM > Hosted Server --- ///
            services.AddHttpClient("local", (serviceProvider, client) =>
            {
                var navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
                client.BaseAddress = new Uri(navigationManager.BaseUri);
            })
            //.AddHttpMessageHandler<BlazorDisplaySpinnerAutomaticallyHttpMessageHandler>()
            .ConfigurePrimaryHttpMessageHandler(sp => (HttpMessageHandler)sp.GetService(MonoWasmHttpMessageHandlerType));

            services.AddTransient<HttpClient>(sp => sp.GetService<IHttpClientFactory>().CreateClient("local"));
        }

        public void ConfigureApi(IServiceCollection services)
        {
            services.AddHttpClient("api", client =>
            {
                client.BaseAddress = new Uri("https://localhost:44340/");
            })
           .AddHttpMessageHandler<BlazorDisplaySpinnerAutomaticallyHttpMessageHandler>()
           .ConfigurePrimaryHttpMessageHandler(sp => (HttpMessageHandler)sp.GetService(MonoWasmHttpMessageHandlerType));

            //services.AddScoped<IEmployeeDataService, MockEmployeeDataService>();
            //services.AddScoped<IEmployeeDataService, EmployeeDataService>();
            //services.AddScoped<ICountryDataService, CountryDataService>();
            //services.AddScoped<IJobCategoryDataService, JobCategoryDataService>();
            //services.AddScoped<IBenefitDataService, BenefitDataService>();

           services.AddScoped<IEmployeeDataService>(sp => new EmployeeDataService(sp.GetService<IHttpClientFactory>().CreateClient("api")));
            services.AddScoped<ICountryDataService>(sp => new CountryDataService(sp.GetService<IHttpClientFactory>().CreateClient("api")));
            services.AddScoped<IJobCategoryDataService>(sp => new JobCategoryDataService(sp.GetService<IHttpClientFactory>().CreateClient("api")));
            services.AddScoped<IBenefitDataService>(sp => new BenefitDataService(sp.GetService<IHttpClientFactory>().CreateClient("api")));
        }

        public void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddHttpClient("idp", client =>
             {
                 client.BaseAddress = new Uri("https://localhost:44333/");
             })
            .ConfigurePrimaryHttpMessageHandler(sp => (HttpMessageHandler)sp.GetService(MonoWasmHttpMessageHandlerType));
            
            services.AddSingleton<IOpenIDConnectService>(sp =>
            new OpenIDConnectService(
                sp.GetService<IHttpClientFactory>().CreateClient("idp"),
                clientId: "bethanyspieshophr_spa",
                responseType: "code",//Authorization Code + PKCE
                scopes: "openid profile email bethanyspieshophrapi country",
                redirectUri: "http://localhost:53779/signin-oidc",
                postLogoutRedirectUri: "http://localhost:53779/signout-callback-oidc")
            );

            services.AddAuthorizationCore(options => {
                options.AddPolicy(
                       BethanysPieShopHRM.Shared.Policies.CanManageEmployees,
                       BethanysPieShopHRM.Shared.Policies.CanManageEmployeesPolicy());
            });

            services.AddScoped<TokenAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<TokenAuthenticationStateProvider>());
        }
    }
}
