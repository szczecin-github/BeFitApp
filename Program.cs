using BeFitApp.Data;
using BeFitApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization; // Required for language
using System.Globalization; // Required for language

var builder = WebApplication.CreateBuilder(args);

// 1. Add Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "DataSource=befit.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. Add Identity
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 3. Add MVC with Localization Support
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

var app = builder.Build();

// 4. Configure Localization Options (PL and EN)
var supportedCultures = new[] { new CultureInfo("pl"), new CultureInfo("en") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pl"), // Default to Polish
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// 5. Enable the Localization Middleware (Must be before Routing)
app.UseRequestLocalization(localizationOptions);

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TrainingSessions}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();