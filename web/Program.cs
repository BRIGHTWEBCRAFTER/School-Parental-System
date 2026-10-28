using SchoolParentalSystem.Web.Filters;
using SchoolParentalSystem.Web.Services.SuperAdmin;
using Microsoft.AspNetCore.Authentication.Cookies;
using Supabase.Extensions.DependencyInjection;
using SchoolParentalSystem.Web.Services;
using SchoolParentalSystem.Web.Services.Teacher;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ActiveUserFilter>();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.AddService<ActiveUserFilter>();
});


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
builder.Services.AddScoped<SchoolAdminParentService>();
builder.Services.AddScoped<SchoolAdminClassService>();
builder.Services.AddScoped<SchoolAdminAttendanceService>();
builder.Services.AddScoped<SchoolAdminGradeService>();
builder.Services.AddScoped<SchoolAdminCommunicationService>();
builder.Services.AddScoped<SchoolAdminProfileService>();
builder.Services.AddScoped<SchoolAdminSearchService>();

builder.Services.AddScoped<SchoolParentalSystem.Web.Services.Teacher.TeacherDashboardService>();
builder.Services.AddScoped<SchoolParentalSystem.Web.Services.Teacher.TeacherAssignedClassesService>();
builder.Services.AddScoped<SchoolParentalSystem.Web.Services.Teacher.TeacherAttendanceService>();
builder.Services.AddScoped<SchoolParentalSystem.Web.Services.Teacher.TeacherGradesService>();
builder.Services.AddScoped<SchoolParentalSystem.Web.Services.Teacher.TeacherCommunicationService>();
builder.Services.AddScoped<SchoolParentalSystem.Web.Services.Teacher.TeacherEventsService>();
builder.Services.AddScoped<SchoolParentalSystem.Web.Services.Teacher.TeacherProfileService>();



// Super Admin services (merged into existing School Admin + Teacher app).
builder.Services.AddScoped<SuperAdminDashboardService>();
builder.Services.AddScoped<SuperAdminSchoolService>();
builder.Services.AddScoped<SuperAdminCommunicationService>();
builder.Services.AddScoped<SuperAdminEventService>();
builder.Services.AddScoped<SuperAdminUserService>();
builder.Services.AddScoped<SuperAdminReportsService>();
builder.Services.AddScoped<SuperAdminProfileService>();
builder.Services.AddScoped<SuperAdminAccessCodeService>();
builder.Services.AddScoped<RegistrationCodeEmailService>();
builder.Services.AddScoped<SchoolAdminRegistrationService>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

//
// Method Name      : AddHttpClient
// Purpose          : Enable secure HTTP communication
//                    with Supabase Authentication.
// Re-use           : Account activation and PIN verification.
// Input Parameters : None.
// Output Type      : IServiceCollection.
//

builder.Services.AddHttpClient();


//
// Service Name : ParentCommunicationEmailService
// Purpose      : Sends announcement and event
//                emails to eligible Parents.
// Re-use       : Teacher and School Admin portals.
//

builder.Services.AddScoped<ParentCommunicationEmailService>();


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

