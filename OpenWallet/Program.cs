using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;
using OpenWallet.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<UserContext>(options => options.UseSqlServer(connectionString));

builder.Services
    .AddDefaultIdentity<UserCustom>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.SignIn.RequireConfirmedEmail = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<UserContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddAuthorization(options =>
{
    foreach (var permission in OpenWalletConstants.Permissions)
    {
        options.AddPolicy(permission, policy => policy.Requirements.Add(new PermissionRequirement(permission)));
    }
});

builder.Services.AddLocalization();
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var cultures = new[] { new CultureInfo("en-US"), new CultureInfo("ar-SA") };
    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
});

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, NoOpEmailSender>();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRequestLocalization();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

try
{
    await OpenWalletSeeder.SeedAsync(app.Services);
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "OpenWallet database seeding was skipped because the configured SQL Server is not reachable.");
}

app.Run();
