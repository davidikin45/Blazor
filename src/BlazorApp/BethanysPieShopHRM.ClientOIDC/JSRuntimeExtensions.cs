using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.ClientOIDC
{
    public static class JSRuntimeExtensions
    {
        public static ValueTask<bool> Confirm(this IJSRuntime jsRuntime, string message)
            => jsRuntime.InvokeAsync<bool>("confirm", message);
     
        public static ValueTask<bool> ContainKeyAsync(this IJSRuntime jsRuntime, string key) 
            => jsRuntime.InvokeAsync<bool>("localStorage.hasOwnProperty", key);

        public static async ValueTask<T> GetItemAsync<T>(this IJSRuntime jsRuntime, string key)
        {
            var serialisedData = await jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

            if (string.IsNullOrWhiteSpace(serialisedData))
                return default(T);

            return JsonSerializer.Deserialize<T>(serialisedData, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public static ValueTask SetItemAsync(this IJSRuntime jsRuntime, string key, object value)
            => jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));

        public static ValueTask RemoveItemAsync(this IJSRuntime jsRuntime, string key)
            => jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);

        public static ValueTask ClearAsync(this IJSRuntime jsRuntime)
         => jsRuntime.InvokeVoidAsync("localStorage.clear");
    }
}
