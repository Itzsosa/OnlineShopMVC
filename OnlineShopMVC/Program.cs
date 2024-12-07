using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using OnlineShopMVC.Data;
using OnlineShopMVC.Models;
using OnlineShopMVC.Services;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddControllersWithViews();

//Configura la autenticación y autorización con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/Login"; // Página de inicio de sesión
        options.AccessDeniedPath = "/Home/AccessDenied"; // Página de acceso denegado (si deseas usarla)
    });

builder.Services.AddControllersWithViews(options =>
{
    
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OnlineShopDB_Connection")));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Manejar errores de estado como el 404
app.UseStatusCodePagesWithReExecute("/Home/NotFound", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
