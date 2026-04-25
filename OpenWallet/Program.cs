using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenWallet.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Point to DefaultConnection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. Configure the Context to use your SQL Server string
builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<UserCustom>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<UserContext>();

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();