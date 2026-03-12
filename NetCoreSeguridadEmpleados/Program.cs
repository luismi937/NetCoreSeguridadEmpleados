using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NetCoreSeguridadEmpleados.Data;
using NetCoreSeguridadEmpleados.Policies;
using NetCoreSeguridadEmpleados.Repositories;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HospitalContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<RepositoryHospital>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, config =>
{
    config.LoginPath = "/Managed/Login";
    config.LogoutPath = "/Managed/Logout";
    config.AccessDeniedPath = "/Managed/ErrorAcceso";
});

builder.Services.AddSingleton<IAuthorizationHandler, OverSalarioRequirement>();
builder.Services.AddSingleton<IAuthorizationHandler, HasSubordinatesRequirement>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmpleadoPolicy", policy =>
    {
        policy.RequireRole("PRESIDENTE", "DIRECTOR", "ANALISTA");
    });
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireClaim("Admin");
    });
    options.AddPolicy("SalarioPolicy", policy =>
    {
        policy.AddRequirements(new OverSalarioRequirement());
    });
    options.AddPolicy("SubordinatesPolicy", policy =>
    {
        policy.AddRequirements(new HasSubordinatesRequirement());
    });
});

builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapStaticAssets();


app.Run();
