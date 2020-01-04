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
* SignalR connection kept open

## Client-side
* Runs on all modern browsers
* No .NET required on server
* SPA user experience
* Older browsers might not be supported
* Initial app downloaded is larger

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