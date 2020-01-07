using System;
using System.Net.Http;
using System.Reflection;
using BethanysPieShopHRM.Server;
using BethanysPieShopHRM.Server.Interceptors;
using BethanysPieShopHRM.Server.Services;
using Microsoft.AspNetCore.Blazor.Http;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BethanysPieShopHRM.ClientApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            //HttpClient Factory does not work with Server side blazor
            services.AddScoped<HttpClient>(s =>
            {
                var blazorDisplaySpinnerAutomaticallyHttpMessageHandler = s.GetRequiredService<BlazorDisplaySpinnerAutomaticallyHttpMessageHandler>();
                blazorDisplaySpinnerAutomaticallyHttpMessageHandler.InnerHandler = new WebAssemblyHttpMessageHandler();

                var client = new HttpClient(blazorDisplaySpinnerAutomaticallyHttpMessageHandler) { BaseAddress = new System.Uri("https://localhost:44340/") };
                return client;
            });

            //services.AddScoped<IEmployeeDataService, MockEmployeeDataService>();
            services.AddScoped<IEmployeeDataService, EmployeeDataService>();
            services.AddScoped<ICountryDataService, CountryDataService>();
            services.AddScoped<IJobCategoryDataService, JobCategoryDataService>();

            //Spinner
            services.AddScoped<ISpinnerService, SpinnerService>();
            services.AddScoped<BlazorDisplaySpinnerAutomaticallyHttpMessageHandler>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
