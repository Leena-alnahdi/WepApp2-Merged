using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using WepApp2.Data;

var builder = WebApplication.CreateBuilder(args);

// ✅ ربط الـ DbContext بقاعدة البيانات
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ MVC + Razor Views
builder.Services.AddControllersWithViews();

// ✅ تفعيل المصادقة باستخدام الاسم الافتراضي "Cookies"
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Auth/Login";               // صفحة تسجيل الدخول
        options.AccessDeniedPath = "/Auth/AccessDenied"; // صفحة الرفض إذا لزم
    });

// ✅ إضافة صلاحيات (authorization)
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ تفعيل المصادقة والصلاحيات
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Dashboard}/{action=Index}/{id?}");
});

app.Run();
