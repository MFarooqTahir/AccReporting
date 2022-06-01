global using AccReporting.Shared.ContextModels;
using AccReporting.Server.Data;
using AccReporting.Server.DbContexts;
using AccReporting.Server.OptimizedModels;
using AccReporting.Server.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Serilog;
using Serilog.Events;

var builder = WebApplication
    .CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning));
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionString1 = builder.Configuration.GetConnectionString("NoDb");
var connectionString2 = builder.Configuration.GetConnectionString("test");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseModel(ApplicationDbContextModel.Instance);
});

builder.Services.AddDbContext<AccountInfoDbContext>(options =>
{
    options.UseSqlServer(connectionString2);
    options.UseModel(AccountInfoDbContextModel.Instance);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)

    .AddRoles<IdentityRole>()
    //.AddClaimsPrincipalFactory<MyUserClaimsPrincipalFactory>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            });
builder.Services.AddDistributedMemoryCache();
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
}); ;
builder.Services.AddRazorPages();
builder.Services.AddScoped<DataService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();

app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();