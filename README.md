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
* Use public RenderFragment ChildContent { get; set; } for child content
* RenderFragment is html to be rendered
* Blazor doesn't support complex data model property validation. Install Microsoft.AspNetCore.Blazor.DataAnnotations.Validation and use [ValidateComplexType]

## Server-side
* Small download
* Works with all server-side APIs
* Full debugging support
* Blazor apps in non-supported browsers
* No offline support
* Network delay
* Scalability, although not a big problem

## Server-side DI
* Transient is scoped to the lifetime of the Blazor component which only occurs on navigation. Use IServiceScopeFactory for true transient.
* Scoped is per application instance.
* Singleton is per application.

## Client-side DI
* Transient is scoped to the lifetime of the Blazor component which only occurs on navigation. Use IServiceScopeFactory for true transient.
* Scoped is per application instance.
* Singleton is per application instance.

## Server-side security
* SignalR connection kept open
* Cookie authentication allows existing user credentials to flow to signalR connections
* @attribute [Authorize(Policy = BethanysPieShopHRM.Shared.Policies.CanManageEmployees )] on pages and  <AuthorizeView Policy="@BethanysPieShopHRM.Shared.Policies.CanManageEmployees"> for child components within page
* ASP.NET Core Identity Deep Dive
* Install [IdentityServer4.Quickstart.UI](https://github.com/IdentityServer/IdentityServer4.Quickstart.UI)
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
* Have built-in support for loading configuration data from appsettings.json and environment specific configuration data from appsettings.{environment}.json.

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
<p>@ChildContent</p>
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
		
		[Parameter]
        public RenderFragment ChildContent { get; set; }
		
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
* OnInitializedAsync - JSRuntime available Web Assembly/Server
* OnParametersSetAsync - JSRuntime available Web Assembly/Server
* OnAfterRenderAsync - JSRuntime available Web Assembly/Server/ServerPrerendered

## Input Components
* InputText
* InputTextArea
* InputNumber 
* InputSelect
* InputData
* InputCheckbox

## Events
* @onclick

## Pass Object
```
var blazorInterop = blazorInterop || {};

blazorInterop.logToConsoleTable = function (obj) {
	console.table(obj);
}
```

```
@inject IJSRuntime JSRuntime
@code{
	protected override async Task OnInitializedAsync()
    {
        await JSRuntime.InvokeVoidAsync("blazorInterop.logToConsoleTable", new Employee { FirstName = "David", LastName = "Ikin"}); 
    }
}
```

## Return string
```
var blazorInterop = blazorInterop || {};

blazorInterop.showPrompt = function (message, defaultValue) {
	return prompt(message, defaultValue);
}
```

```
@inject IJSRuntime JSRuntime

<button @onclick="ShowPrompt">show prompt</button>
@code{
	private string promptResult;
	
	private async Task ShowPrompt()
    {
        var result = await JSRuntime.InvokeAsync<string>("blazorInterop.showPrompt", "What's your name?", promptResult ?? ""); 
		if(result != null)
		{
			promptResult = result;
		}
    }
}
```

## Return Object
```
var blazorInterop = blazorInterop || {};

blazorInterop.createEmployee = function (firstName, lastName) {
	return { firstName, lastName, email: firstName + "@gmail.com"};
}
```

```
@inject IJSRuntime JSRuntime

<button @onclick="GetEmployee">Get Employee</button>
@code{
	private Employee employee;
	
	private async Task GetEmployee()
    {
        employee = await JSRuntime.InvokeAsync<Employee>("blazorInterop.createEmployee", "David"); 
    }
}
```

## Reference
* Can't be used in OnInitializedAsync or OnParametersSetAsync
```
var blazorInterop = blazorInterop || {};

blazorInterop.focusElement = function (element) {
	element.focus;
}

blazorInterop.focusElementById = function (id) {
	var elemeent = document.getElementById(id);
	if(element) element.focus();
}
```

```
@inject IJSRuntime JSRuntime
<input @ref="elementToFocus" type="text" />
@code{
	private ElementReference elementToFocus;
	
	protected async override Task OnAfterRenderAsync(bool firstRender)
    {
		if (firstRender)
		{
			await JSRuntime.InvokeVoidAsync("blazorInterop.focusElement", elementToFocus); 
		}
    }
}
```

## Errors
```
var blazorInterop = blazorInterop || {};

blazorInterop.throwsError = function () {
	throw Error("An error has occured");
}
```

```
@inject IJSRuntime JSRuntime
@code{
	private string errorMessage;

	protected async override Task OnAfterRenderAsync(bool firstRender)
    {
		if (firstRender)
		{
			try
			{
				await JSRuntime.InvokeVoidAsync("blazorInterop.throwsError"); 	
			}
			catch(JSExpcetion exception)
			{
				errorMessage = exception.Message;
			}
		}
    }
}
```

## Call C# static method
```
var blazorInterop = blazorInterop || {};

blazorInterop.callStaticDotNetMethod = function () {
	var promise = DotNet.invokceMethodAsync("BethanysPieShopHRM.ServerApp", "BuildEmail", "David");
	promise.then(email => alert(email));
}
```

```
@inject IJSRuntime JSRuntime
<button onclick="blazorInterop.callStaticDotNetMethod()">Build Email</button>
@code{
	[JSInvokable]
	public static string BuildEmail(string firstName)
	{
		return $"{firstName}@gmail.com";
	}
}
```


## Call C# instance method
```
var blazorInterop = blazorInterop || {};

blazorInterop.callDotNetInstanceMethod = function (dotNetObjectRef) {
	dotNetObjectRef.invokeMethodAsync("SetWindowSize", {
		width: window.innerWidth,
		height: window.innerHeight
	});
}
```

```
@inject IJSRuntime JSRuntime
<button @onclick="PassDotNetInstanceToJavaScript">Call It</button>
@code{
	private Size _windowSize;
	
	private async Task PassDotNetInstanceToJavaScript()
	{
		var dotNetObjectReference = DotNetObjectReference.Create(this);
		await JSRuntime.InvokeVoidAsync("blazorIntercop.callDotNetInstanceMethod", dotNetObjectReference);
	}

	[JSInvokable]
	public void SetWindowSize(Size windowSize)
	{
		_windowSize = windowSize;
		StateHasChanged();
	}
}
```

## Call C# instance method from JS Event Handler
```
var blazorInterop = blazorInterop || {};

blazorInterop.registerResizeHandler = function (dotNetObjectRef) {
	function resizeHandler()
	{
		dotNetObjectRef.invokeMethodAsync("SetWindowSize", {
			width: window.innerWidth,
			height: window.innerHeight
		});
	};
	
	resizeHandler();
	
	window.addEventListener("resize", resizeHandler);
}
```

```
@inject IJSRuntime JSRuntime
@code{
	private Size _windowSize;
	
	protected async override Task OnAfterRenderAsync(bool firstRender)
    {
		if (firstRender)
		{
			var dotNetObjectReference = DotNetObjectReference.Create(this);
			await JSRuntime.InvokeVoidAsync("blazorIntercop.registerResizeHandler", dotNetObjectReference);
		}
    }

	[JSInvokable]
	public void SetWindowSize(Size windowSize)
	{
		_windowSize = windowSize;
		StateHasChanged();
	}
}
```

## Online Handler
```
var blazorInterop = blazorInterop || {};

blazorInterop.registerOnlineHandler = function (dotNetObjectRef) {
  function onlineHandler() {
    dotNetObjectRef.invokeMethodAsync("SetOnlineStatus",
      navigator.onLine);
  };

  // Set up initial values
  onlineHandler();

  // Register event handler
  window.addEventListener("online", onlineHandler);
  window.addEventListener("offline", onlineHandler);
};
```

```
@inject IJSRuntime JSRuntime
@code{
	private bool _isOnline;
	
	protected async override Task OnAfterRenderAsync(bool firstRender)
    {
		if (firstRender)
		{
			var dotNetObjectReference = DotNetObjectReference.Create(this);
			await JSRuntime.InvokeVoidAsync("blazorIntercop.registerOnlineHandler", dotNetObjectReference);
		}
    }

	[JSInvokable]
	public void SetOnlineStatus(bool isOnline)
	{
		_isOnline = isOnline;
		StateHasChanged();
	}
}
```

## Local Storage
```
services.AddTransient<ILocalStorageService, LocalStorageService>();
public interface ILocalStorageService
{
Task SetItemAsync<T>(string key, T item);

Task<T> GetItemAsync<T>(string key);
}
  
public class LocalStorageService : ILocalStorageService
{
private readonly IJSRuntime _jsRuntime;

public LocalStorageService(IJSRuntime jsRuntime)
{
  _jsRuntime = jsRuntime;
}

public async Task SetItemAsync<T>(string key, T item)
{
  await _jsRuntime.InvokeVoidAsync("localStorage.setItem",
	key, JsonSerializer.Serialize(item));
}

public async Task<T> GetItemAsync<T>(string key)
{
  var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
  return string.IsNullOrEmpty(json)
	? default
	: JsonSerializer.Deserialize<T>(json);
}
}
```

## Pluralsight Courses
* [Blazor: Getting Started](https://app.pluralsight.com/library/courses/getting-started-blazor/table-of-contents)
* [Creating Blazor Components](https://app.pluralsight.com/library/courses/creating-blazor-components/table-of-contents)
* [Authentication and Authorization in Blazor Applications](https://app.pluralsight.com/library/courses/authentication-authorization-blazor-applications/table-of-contents)
* [Using HttpClient to Consume APIs in .NET Core](https://app.pluralsight.com/library/courses/httpclient-consume-apis-dotnet-core/table-of-contents)

## Posts
* [Blazor WebAssembly 3.2.0 Preview 2 release now available](https://devblogs.microsoft.com/aspnet/blazor-webassembly-3-2-0-preview-2-release-now-available/?fbclid=IwAR09ld1IMqbkWg-pDcYvDcQteGRChuvIc1XAKY-jKEQ8mUorGLVPU2Z1Ki8)
* [Blazor WebAssembly 3.2.0 Preview 3 release now available](https://devblogs.microsoft.com/aspnet/blazor-webassembly-3-2-0-preview-3-release-now-available/)