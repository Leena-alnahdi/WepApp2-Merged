using Microsoft.AspNetCore.Mvc;
using WepApp2.Data;
using WepApp2.Models;
using System.Linq;

namespace WepApp2.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        // عرض صفحة الملف الشخصي
        public IActionResult Profile()
        {
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Auth");

            var user = _context.Users.FirstOrDefault(u => u.UserName == username);

            if (user == null)
                return NotFound();

            return View("Profile", user);
        }

        // تحديث رقم الهاتف
        [HttpPost]
        public IActionResult UpdatePhone([FromBody] User model)
        {
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var user = _context.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
                return NotFound();

            user.PhoneNumber = model.PhoneNumber;
            _context.SaveChanges();

            return Ok();
        }
    }
}
