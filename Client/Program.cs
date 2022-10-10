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

var builder = WebAssemblyHostBuilder.CreateDefault(args: args);
builder.RootComponents.Add<App>(selector: "#app");
builder.RootComponents.Add<HeadOutlet>(selector: "head::after");
builder.Services.AddSyncfusionBlazor();
//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetValue<string>("SyncfusionLicenseKey"));

builder.Services.AddHttpClient(name: "AccReporting.ServerAPI", configureClient: client => client.BaseAddress = new Uri(uriString: builder.HostEnvironment.BaseAddress))
    .ConfigureHttpClient(configureClient: x => x.DefaultRequestHeaders.Accept
    .Add(item: new MediaTypeWithQualityHeaderValue(mediaType: "application/json")));
//.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
builder.Services.AddLoadingBar();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(implementationFactory: sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(name: "AccReporting.ServerAPI").EnableIntercept(services: sp));
//var client = builder.Services.BuildServiceProvider().GetRequiredService<HttpClient>();
//var Cal = await client.GetByteArrayAsync("Fonts/Calibri/Calibri.ttf");
//var Calb = await client.GetByteArrayAsync("Fonts/Calibri/calibrib.ttf");
//var Call = await client.GetByteArrayAsync("Fonts/Calibri/calibril.ttf");
//FontManager.RegisterFont(new MemoryStream(Cal));
//FontManager.RegisterFont(new MemoryStream(Calb));
//FontManager.RegisterFont(new MemoryStream(Call));

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddHttpClientInterceptor();
builder.Services.AddScoped<HttpInterceptorService>();
var culture = new CultureInfo(name: "hi-IN");
culture.NumberFormat.CurrencySymbol = "Rs.";
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
builder.UseLoadingBar();
await builder.Build().RunAsync();