using BethanysPieShopHRM.Api.Models;
using FileContextCore;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BethanysPieShopHRM.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseFileContextDatabase(databaseName: "db").Options);
            //context.Employees.Add(new Employee() { Gender = Gender.Male });
            //context.SaveChanges();

            //services.AddDbContext<AppDbContext>(options =>
            //{
            //    options.UseInMemoryDatabase(databaseName: "BethanysPieShopHRM");
            //});

            //services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection")));

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });

            //services.AddDbContext<AppDbContext>(options => options.UseFileContextDatabase(databaseName:"db"));

            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IJobCategoryRepository, JobCategoryRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IBenefitRepository, BenefitRepository>();

            services.AddCors(options =>
            {
                options.AddPolicy("Open", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddControllers();
            //.AddJsonOptions(options => options.JsonSerializerOptions.ca);

            //Remote JWT Authentication
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:44333/";
                    options.ApiName = "bethanyspieshophrapi";
                });

            services.AddAuthorization(options =>
            {
                //ALL Endpoints which dont have authorization defined!
                //options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

                options.AddPolicy(
                   BethanysPieShopHRM.Shared.Policies.CanManageEmployees,
                   BethanysPieShopHRM.Shared.Policies.CanManageEmployeesPolicy());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("Open");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
