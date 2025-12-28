using BoxFortQuoteRecap_2025;
using BoxFortQuoteRecap_2025.Application;
using BoxFortQuoteRecap_2025.Infrastructure;
using BoxFortQuoteRecap_2025.Infrastructure.Configuration;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Wahhhhh, I'm WASM, I don't have native support for an appsettings file, wahhhhh

using var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
using var response = await http.GetAsync("appsettings.json");
using var stream = await response.Content.ReadAsStreamAsync();
builder.Configuration.AddJsonStream(stream);

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection(nameof(AppSettings))
);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddApplication();
await builder.Services.AddInfrastructureAsync(builder.Configuration, builder.HostEnvironment.BaseAddress);
await builder.Build().RunAsync();
