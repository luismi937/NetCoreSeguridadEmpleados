using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NetCoreSeguridadEmpleados.Data;
using NetCoreSeguridadEmpleados.Policies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//las politicas se agregan con Authorization
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
});


string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<HospitalContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

}).AddCookie(
    CookieAuthenticationDefaults.AuthenticationScheme, config =>
    {

        config.AccessDeniedPath = "/Managed/ErrorAcceso";

    }
    );
builder.Services.AddControllersWithViews(options =>
{
    options.EnableEndpointRouting = false;
}).AddSessionStateTempDataProvider();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();
app.UseSession();

app.MapStaticAssets();

app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "default",
        template: "{controller=Home}/{action=Index}/{id?}");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
