using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using WepApp2.Data;
using WepApp2.Models;

namespace WepApp2.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Login - دالة واحدة فقط
        [HttpGet]
        public IActionResult Login()
        {
            // تمرير TempData إلى ViewBag
            if (TempData["ResetSuccess"] != null)
            {
                ViewBag.ResetSuccess = true;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            var existingUser = _context.Users
            .FirstOrDefault(u => u.UserName == user.UserName && u.UserPassWord == user.UserPassWord);

            if (existingUser != null)
            {
                // إنشاء قائمة الـ Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, existingUser.UserName),
                    new Claim(ClaimTypes.Role, existingUser.UserRole ?? "Student"),
    new Claim("FirstName", existingUser.FirstName ?? "")  // ✅ هنا نضيف الاسم الأول
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // تسجيل الدخول بالكوكيز
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // التوجيه حسب الدور
                if (existingUser.UserRole == "Supervisor")
                    return RedirectToAction("Index", "Supervisor");

                return RedirectToAction("HomePage", "Auth");
            }

            ViewBag.LoginFailed = true;
            return View(user);
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user, string ConfirmPassword)
        {
            if (user.UserPassWord != ConfirmPassword)
            {
                ViewBag.PasswordMismatch = true;
                return View(user);
            }

            if (string.IsNullOrWhiteSpace(user.UserPassWord))
            {
                ViewBag.PasswordEmpty = true;
                return View(user);
            }

            var existingUser = _context.Users
                .FirstOrDefault(u => u.UserName == user.UserName || u.Email == user.Email);

            if (existingUser != null)
            {
                ViewBag.UserExists = true;
                return View(user);
            }

            if (ModelState.IsValid)
            {
                user.LastLogIn = DateTime.Now;
                user.IsActive = true;

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(user);
        }

        // Dashboard view
        [HttpGet]
        public IActionResult HomePage()
        {
            return View();
        }

        // GET: Forgot Password
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Forgot Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            // التحقق من أن البريد ليس فارغاً
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = true;
                ViewBag.ErrorMessage = "يرجى إدخال البريد الإلكتروني";
                return View();
            }

            // التحقق من أن البريد ينتمي للجامعة
            if (!email.EndsWith("@kau.edu.sa") && !email.EndsWith("@stu.kau.edu.sa"))
            {
                ViewBag.Error = true;
                ViewBag.ErrorMessage = "يجب استخدام البريد الإلكتروني الجامعي (@kau.edu.sa أو @stu.kau.edu.sa)";
                return View();
            }

            // البحث عن المستخدم في قاعدة البيانات
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                // البريد موجود - استخدام TempData
                TempData["ResetSuccess"] = "true";

                return RedirectToAction("Login");
            }
            else
            {
                // البريد غير موجود
                ViewBag.Error = true;
                ViewBag.ErrorMessage = "البريد الإلكتروني غير مسجل في النظام، تأكد من صحة البريد الإلكتروني";
                return View();
            }
        }

        // تسجيل الخروج
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}