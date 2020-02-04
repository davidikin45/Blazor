using BethanysPieShopHRM.Server;
using Microsoft.AspNetCore.Blazor.Hosting;
using System.Threading.Tasks;

namespace BethanysPieShopHRM.ClientApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            new Startup().ConfigureServices(builder.Services);
            builder.RootComponents.Add<ClientAppWithAuth>("app");

            var host = builder.Build();


            await host.RunAsync();
        }
    }
}
