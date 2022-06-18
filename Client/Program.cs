using AccReporting.Client;
using AccReporting.Client.Classes;
using AccReporting.Client.Services;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Syncfusion.Blazor;

using System.Globalization;
using System.Net.Http.Headers;

using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSyncfusionBlazor();
//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetValue<string>("SyncfusionLicenseKey"));

builder.Services.AddHttpClient("AccReporting.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .ConfigureHttpClient(x => x.DefaultRequestHeaders.Accept
    .Add(new MediaTypeWithQualityHeaderValue("application/json")));
//.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
builder.Services.AddLoadingBar();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("AccReporting.ServerAPI").EnableIntercept(sp));
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddHttpClientInterceptor();
builder.Services.AddScoped<HttpInterceptorService>();
var culture = new CultureInfo("hi-IN");
culture.NumberFormat.CurrencySymbol = "Rs.";
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
builder.UseLoadingBar();
await builder.Build().RunAsync();