// SupervisorController.cs (بعد التعديلات)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WepApp2.Data;
using WepApp2.Models;

namespace WepApp2.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly AppDbContext _context;

        public SupervisorController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var requests = _context.Requests
                                   .Include(r => r.User)
                                   .Include(r => r.Device)
                                   .ToList();
            return View(requests);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            var req = _context.Requests.FirstOrDefault(r => r.RequestId == request.RequestId);
            if (req != null)
            {
                req.SupervisorStatus = request.Status;
                if (!string.IsNullOrEmpty(request.Notes))
                {
                    req.Notes = request.Notes;
                }
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public IActionResult GetVisitType(int id)
        {
            var request = _context.Requests
                                  .Include(r => r.LabVisits)
                                  .ThenInclude(lv => lv.VisitDetails)
                                  .FirstOrDefault(r => r.RequestId == id);

            if (request == null || request.RequestType != "زيارة معمل")
            {
                return Json(new { visitType = "غير متاح" });
            }

            var visitType = request.LabVisits.FirstOrDefault()?.VisitDetails?.VisitType ?? "غير متاح";
            return Json(new { visitType });
        }

        [HttpGet]
        public IActionResult GetConsultationDescription(int id)
        {
            var consultation = _context.Consultations
                .Where(c => c.RequestId == id)
                .Select(c => new { consultationDescription = c.ConsultationDescription })
                .FirstOrDefault();

            return Json(consultation ?? new { consultationDescription = "غير متاح" });
        }

        [HttpGet]
        public IActionResult GetCourseName(int id)
        {
            var course = _context.Courses
                .Where(c => c.RequestId == id)
                .Select(c => new { courseName = c.CourseName })
                .FirstOrDefault();

            return Json(course ?? new { courseName = "غير متاح" });
        }

        public class UpdateStatusRequest
        {
            public int RequestId { get; set; }
            public string Status { get; set; }
            public string? Notes { get; set; }
        }
    }
}