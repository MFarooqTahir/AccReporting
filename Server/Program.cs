global using AccReporting.Shared.ContextModels;
using AccReporting.Server;
using AccReporting.Server.Data;
using AccReporting.Server.DbContexts;
using AccReporting.Server.OptimizedModels;
using AccReporting.Server.Services;

using HashidsNet;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using QuestPDF.Drawing;

using Serilog;
using Serilog.Events;

using System.Globalization;
using System.Text;

var builder = WebApplication
    .CreateBuilder(args: args);
var culture = new CultureInfo(name: "hi-IN");
culture.NumberFormat.CurrencySymbol = "Rs.";
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

builder.Host.UseSerilog(configureLogger: (ctx, lc) => lc
    .WriteTo.Console()
    .MinimumLevel.Override(source: "Microsoft.AspNetCore", minimumLevel: LogEventLevel.Warning));
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString(name: "DefaultConnection");
var connectionString1 = builder.Configuration.GetConnectionString(name: "NoDb");
var connectionString2 = builder.Configuration.GetConnectionString(name: "test");

builder.Services.AddSingleton<IHashids>(implementationInstance: new Hashids(salt: "OzoneTechnologies softwares avax", minHashLength: 6));
builder.Services.AddTransient<IEmailSender, EmailService>();
builder.Services.AddDbContext<ApplicationDbContext>(optionsAction: options =>
{
    options.UseMySql(connectionString: connectionString, serverVersion: ServerVersion.AutoDetect(connectionString: connectionString));
    options.UseModel(model: ApplicationDbContextModel.Instance);
});

builder.Services.AddDbContext<AccountInfoDbContext>(optionsAction: options =>
{
    options.UseMySql(connectionString: connectionString2, serverVersion: ServerVersion.AutoDetect(connectionString: connectionString2));
    options.UseModel(model: AccountInfoDbContextModel.Instance);
});
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(configureOptions: options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    //.AddClaimsPrincipalFactory<MyUserClaimsPrincipalFactory>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthentication(configureOptions: opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(configureOptions: options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration[key: "validIssuer"],
        ValidAudience = builder.Configuration[key: "validAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(key: Encoding.UTF8.GetBytes(s: builder.Configuration[key: "securityKey"]))
    };
});
builder.Services.AddControllersWithViews().AddNewtonsoftJson(setupAction: options => options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver());
builder.Services.AddDistributedMemoryCache();
builder.Services.AddCors(setupAction: options => options.AddPolicy(name: "MyPolicy", configurePolicy: builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
builder.Services.AddRazorPages();
builder.Services.AddScoped<DataService>();

var app = builder.Build();
var hostingEnvironment = app.Services.GetService<IWebHostEnvironment>();

var filePaths = Directory.GetFiles(path: Path.Combine(path1: hostingEnvironment.WebRootPath, path2: "Fonts/Calibri"));
foreach (var filepath in filePaths)
{
    using var fs = new FileStream(path: filepath, mode: FileMode.Open, access: FileAccess.Read);
    FontManager.RegisterFont(stream: fs);
}
filePaths = Directory.GetFiles(path: Path.Combine(path1: hostingEnvironment.WebRootPath, path2: "Fonts/Fira"));
foreach (var filepath in filePaths)
{
    using var fs = new FileStream(path: filepath, mode: FileMode.Open, access: FileAccess.Read);
    FontManager.RegisterFont(stream: fs);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler(errorHandlingPath: "/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
var cookiePolicyOptions = new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
};
app.UseCookiePolicy(options: cookiePolicyOptions);
//app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile(filePath: "index.html");

app.Run();