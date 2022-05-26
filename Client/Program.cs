﻿using AccReporting.Client;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Syncfusion.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddSyncfusionBlazor();

//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetValue<string>("SyncfusionLicenseKey"));

builder.Services.AddHttpClient("AccReporting.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("AccReporting.ServerAPI"));
var client = builder.Services.BuildServiceProvider().GetRequiredService<HttpClient>();
var fontcal = await client.GetByteArrayAsync("fonts/Calibri/Calibri.ttf");
var fontcalb = await client.GetByteArrayAsync("fonts/Calibri/calibrib.ttf");
var fontcall = await client.GetByteArrayAsync("fonts/Calibri/calibril.ttf");

QuestPDF.Drawing.FontManager.RegisterFont(new MemoryStream(fontcal));
QuestPDF.Drawing.FontManager.RegisterFont(new MemoryStream(fontcalb));
QuestPDF.Drawing.FontManager.RegisterFont(new MemoryStream(fontcall));

builder.Services.AddApiAuthorization();

await builder.Build().RunAsync();