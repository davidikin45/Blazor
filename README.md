# Blazor
* .razor files
* Razor component must start with Uppercase
* Class generated upon compilation
* _Host.cshtml handles all requests going to blazor app
* Mixed or Code-behind (Counter.razor  + CounterBase.cs)
* Important to extract model to shared library so can be used with API and Blazor
* One-way binding for displaying data @Employee.FirstName
* Two-way binding <input id="lastName" @bind="@Employee.FirstName" />
* By default binding occurs when user taps out of textbox. Can change with @bind-value:event="oninput"
* Use NavigationManager.NavigateTo("/employeeoverview") for navigation
* Add System.ComponentModel.Annotations for validation attributes and use <DataAnnotationsValidator /> and <ValidationSummary />
* StateHasChanged() triggers rerender
* JavaScript interop by injecting IJSRuntime JsRuntime

## Server-side
* Small download
* Works with all server-side APIs
* Full debugging support
* Blazor apps in non-supported browsers
* No offline support
* Network delay
* Scalability, although not a big problem

## Server-side security
* SignalR connection kept open
* Cookie authentication allows existing user credentials to flow to signalR connections
* @attribute [Authorize(Policy = BethanysPieShopHRM.Shared.Policies.CanManageEmployees )] on pages and  <AuthorizeView Policy="@BethanysPieShopHRM.Shared.Policies.CanManageEmployees"> for child components within page
* ASP.NET Core Identity Deep Dive
* Scaffold Identity Add > New Scaffolded Item > Identity and add StatusMessage, RegisterConfirmation, ConfirmEmail and Register. Best to add this to IDP and include IdentityServer4.AspNetIdentity
* Change services.AddControllersWithViews() > services.AddMvc()
* Add  endpoints.MapControllers(), endpoints.MapDefaultControllerRoute(), endpoints.MapRazorPages()
* Copy over AccountController and ExternalController from https://github.com/IdentityServer/IdentityServer4.Templates/tree/master/src/IdentityServer4AspNetIdentity/Quickstart/Account
* OpenID Connect extends and supersedes OAuth2
* Securing ASP.NET Core 2 with OAuth2 and OpenID Connect
* Either use Authorization Code + PKCE or Hybrid
* Microsoft.AspNetCore.Authentication.OpenIdConnect
* IdentityModel
* IdentityServer4.AccessTokenValidation
* Automatically logs into app if logged into identity server under any user 
* Enabling Windows Authentication on IDP allows windows users to login
* Attribute-based Access Control (ABAC) can have alot more complex rules than Role-based Access Control (RBAC)
* Add Microsoft.AspNetCore.Authorization to shared project so authorization policies can be used in Blazor and API

```
[assembly: HostingStartup(typeof(IDP.Areas.Identity.IdentityHostingStartup))]
namespace IDP.Areas.Identity
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
```

```
public class DummyEmailSender : IEmailSender
{
	public Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		return Task.CompletedTask;
	}
}
```

## Client-side
* Runs on all modern browsers
* No .NET required on server
* SPA user experience
* Older browsers might not be supported
* Initial app downloaded is larger
* HttpClient, EF Core

## Example Mixed
```
@page "/counter"

<h1>Counter</h1>

<p>Current count: @currentCount</p>

<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

@code {
    private int currentCount = 0;

    private void IncrementCount()
    {
        currentCount++;
    }
}
```

## Example Code-behind
```
@page "/employeedetail{EmployeeId}"
@inherits EmployeeDetailBase

<h1>@EmployeeId</h1>

```

```
public class EmployeeDetailBase : ComponentBase
{
		[Inject]
        public IEmployeeDataService EmployeeDataService { get; set; }
		
		[Inject]
        public NavigationManager NavigationManager { get; set; }
		
		[Inject]
        public IJSRuntime JsRuntime { get; set; }
		
		[Parameter]
        public string EmployeeId { get; set; }
		
		protected override Task OnInitializedAsync()
        {

            return base.OnInitializedAsync();
        }
}

```

## Send Client-sider exceptions to console
```
services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
```

## Component Lifecycle Methods
* OnInitializedAsync
* OnParametersSetAsync
* OnAfterRenderAsync

## Input Components
* InputText
* InputTextArea
* InputNumber
* InputSelect
* InputData
* InputCheckbox

## Pluralsight Courses
* [Blazor: Getting Started](https://app.pluralsight.com/library/courses/getting-started-blazor/table-of-contents)
* [Creating Blazor Components](https://app.pluralsight.com/library/courses/creating-blazor-components/table-of-contents)
* [Authentication and Authorization in Blazor Applications](https://app.pluralsight.com/library/courses/authentication-authorization-blazor-applications/table-of-contents)
* [Using HttpClient to Consume APIs in .NET Core](https://app.pluralsight.com/library/courses/httpclient-consume-apis-dotnet-core/table-of-contents)