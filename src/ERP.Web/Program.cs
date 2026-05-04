using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ERP.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1. Database Configuration
// Registers the ERPDbContext using SQLite. 
// The database file "erp.db" will be created in the app's working directory.
builder.Services.AddDbContext<ERPDbContext>(options =>
  options.UseSqlite("Data Source=erp.db"));

// 2. Authentication Configuration
// Sets up cookie-based authentication.
// If a user is not authorized, they will be redirected to the Login path.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
  .AddCookie(options => {
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
  });

// 3. MVC Services
// Adds Controllers and Views support to the application.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware Pipeline
// Standard development error handling
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Serve static files (CSS, JS, Images) from wwwroot
app.UseStaticFiles();

// Enable Routing
app.UseRouting();

// Enable Authentication and Authorization Middleware
// Must be placed between UseRouting and UseEndpoints
app.UseAuthentication();
app.UseAuthorization();

// Configure MVC Routes
app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");

// Database Initialization
// Ensures the database schema is created on startup if it doesn't exist.
// Note: This is for development simplicity; Migrations are recommended for production.
using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<ERPDbContext>();
  db.Database.EnsureCreated();
}

app.Run();
