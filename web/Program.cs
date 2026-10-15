using Microsoft.AspNetCore.Authentication.Cookies;
using Supabase.Extensions.DependencyInjection;
using SchoolParentalSystem.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();


// ---------------------------------------------------------
// ASP.NET WEB SESSION
// ---------------------------------------------------------

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(45);

    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;

    options.Cookie.SameSite =
        SameSiteMode.Lax;

    options.Cookie.SecurePolicy =
        CookieSecurePolicy.SameAsRequest;
});


// ---------------------------------------------------------
// ASP.NET AUTH COOKIE
// ---------------------------------------------------------

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";

        options.AccessDeniedPath =
            "/Auth/AccessDenied";

        options.ExpireTimeSpan =
            TimeSpan.FromMinutes(45);

        options.SlidingExpiration = true;

        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;

        options.Cookie.SameSite =
            SameSiteMode.Lax;
    });

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();


// ---------------------------------------------------------
// SUPABASE
// ---------------------------------------------------------

builder.Services.AddSupabase(
    supabaseUrl:
        builder.Configuration["Supabase:Url"]!,

    supabaseKey:
        builder.Configuration["Supabase:Key"]!,

    configureOptions: options =>
    {
        options.AutoConnectRealtime = false;

        // We will manage persistence through ASP.NET.
        options.AutoRefreshToken = false;
    });


// Keeps the Supabase tokens attached to each web session.
builder.Services.AddScoped<SupabaseSessionService>();
builder.Services.AddScoped<SchoolAdminLearnerService>();
builder.Services.AddScoped<SchoolAdminTeacherService>();


var app = builder.Build();


// ---------------------------------------------------------
// HTTP PIPELINE
// ---------------------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();


// Session must be available before authentication-dependent
// controllers use Supabase tokens.
app.UseSession();

app.UseAuthentication();

app.UseAuthorization();


// ---------------------------------------------------------
// DEFAULT PAGE
// ---------------------------------------------------------

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Auth}/{action=Login}/{id?}");

app.Run();