using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WepApp2.Models;
using WepApp2.Data;
using System.Linq;

namespace WepApp2.Controllers
{
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult AllReports()
        {
            return View();
        }

        public IActionResult CreateCustomReport()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCustomReport(CustomReportViewModel model)
        {
            try
            {
                if (model.ReportType == "تقرير الطلبات")
                {
                    // جلب البيانات من قاعدة البيانات
                    var requestsQuery = _context.Requests
                        .Include(r => r.User)
                        .Include(r => r.Service)
                        .Include(r => r.Device)
                        .AsQueryable();

                    // تطبيق فلتر التاريخ إذا تم تحديده
                    if (model.FromDate.HasValue)
                    {
                        requestsQuery = requestsQuery.Where(r => r.RequestDate >= model.FromDate.Value);
                    }
                    if (model.ToDate.HasValue)
                    {
                        requestsQuery = requestsQuery.Where(r => r.RequestDate <= model.ToDate.Value);
                    }

                    // تطبيق فلتر حالة الطلب إذا تم تحديده
                    if (!string.IsNullOrEmpty(model.RequestStatus))
                    {
                        requestsQuery = requestsQuery.Where(r =>
                            r.AdminStatus == model.RequestStatus ||
                            r.SupervisorStatus == model.RequestStatus);
                    }

                    var requests = requestsQuery.ToList();

                    // جلب جميع المشرفين مرة واحدة لتحسين الأداء
                    var supervisorIds = requests.Select(r => r.SupervisorAssigned).Distinct().ToList();
                    var supervisors = _context.Users
                        .Where(u => supervisorIds.Contains(u.UserId))
                        .ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);

                    // تحويل البيانات إلى ViewModel للعرض
                    var reportData = requests.Select(r => new RequestReportViewModel
                    {
                        Id = r.RequestId,
                        المستفيد = GetUserFullName(r.User),
                        نوع_الخدمة = GetServiceName(r),
                        الجهاز = r.Device?.DeviceName ?? "لا يوجد",
                        التاريخ = r.RequestDate.ToString("yyyy-MM-dd"),
                        الوقت = r.RequestDate.ToString("HH:mm"),
                        المشرف_المسند = supervisors.ContainsKey(r.SupervisorAssigned)
                            ? supervisors[r.SupervisorAssigned]
                            : "غير مسند",
                        الحالة = GetRequestStatus(r)
                    }).ToList();

                    ViewBag.ReportTitle = model.ReportTitle;
                    ViewBag.ReportType = model.ReportType;
                    ViewBag.FromDate = model.FromDate?.ToString("yyyy-MM-dd");
                    ViewBag.ToDate = model.ToDate?.ToString("yyyy-MM-dd");
                    ViewBag.SelectedFields = model.Fields ?? new List<string>();

                    return View("PrintReport", reportData);
                }
                else if (model.ReportType == "تقرير الأجهزة")
                {
                    // جلب بيانات الأجهزة من قاعدة البيانات
                    var devicesQuery = _context.Devices.AsQueryable();

                    // تطبيق فلتر حالة الجهاز إذا تم تحديده
                    if (!string.IsNullOrEmpty(model.DeviceStatus))
                    {
                        devicesQuery = devicesQuery.Where(d => d.DeviceStatus == model.DeviceStatus);
                    }

                    var devices = devicesQuery.ToList();

                    // تحويل البيانات إلى DeviceReportViewModel
                    var deviceData = devices.Select(d => new DeviceReportViewModel
                    {
                        Id = d.DeviceId,
                        DeviceName = d.DeviceName,
                        DeviceType = d.DeviceType,
                        Status = d.DeviceStatus
                    }).ToList();

                    ViewBag.ReportTitle = model.ReportTitle;
                    ViewBag.ReportType = model.ReportType;
                    ViewBag.FromDate = model.FromDate?.ToString("yyyy-MM-dd");
                    ViewBag.ToDate = model.ToDate?.ToString("yyyy-MM-dd");

                    return View("PrintReport", deviceData);
                }
                else if (model.ReportType == "تقرير المستخدمين")
                {
                    // جلب بيانات المستخدمين من قاعدة البيانات
                    var usersQuery = _context.Users.AsQueryable();

                    // تطبيق فلتر نوع المستخدم إذا تم تحديده
                    if (!string.IsNullOrEmpty(model.UserType))
                    {
                        usersQuery = usersQuery.Where(u => u.UserRole == model.UserType);
                    }

                    var users = usersQuery.ToList();

                    // تحويل البيانات إلى UserReportViewModel
                    var userData = users.Select(u => new UserReportViewModel
                    {
                        Id = u.UserId,
                        Name = u.FirstName + " " + u.LastName,
                        Username = u.UserName,
                        UserType = u.UserRole,
                        Email = u.Email,
                        Phone = u.PhoneNumber,
                        Status = u.IsActive ? "نشط" : "غير نشط"
                    }).ToList();

                    ViewBag.ReportTitle = model.ReportTitle;
                    ViewBag.ReportType = model.ReportType;
                    ViewBag.FromDate = model.FromDate?.ToString("yyyy-MM-dd");
                    ViewBag.ToDate = model.ToDate?.ToString("yyyy-MM-dd");
                    ViewBag.SelectedFields = model.Fields ?? new List<string>();

                    return View("PrintUserReport", userData);
                }
                else
                {
                    // للتقارير الأخرى - تقرير الخدمات
                    // يمكنك إضافة الكود هنا لاحقاً

                    ViewBag.ReportTitle = model.ReportTitle;
                    ViewBag.ReportType = model.ReportType;
                    ViewBag.FromDate = model.FromDate?.ToString("yyyy-MM-dd");
                    ViewBag.ToDate = model.ToDate?.ToString("yyyy-MM-dd");

                    return View("PrintReport", new List<DeviceReportViewModel>());
                }
            }
            catch (Exception ex)
            {
                // في حالة حدوث أي خطأ، نعود إلى الصفحة مع رسالة خطأ
                TempData["Error"] = "حدث خطأ في إنشاء التقرير: " + ex.Message;
                return RedirectToAction("CreateCustomReport");
            }
        }

        // دالة مساعدة للحصول على الاسم الكامل للمستخدم
        private string GetUserFullName(User user)
        {
            if (user == null)
                return "غير محدد";

            return $"{user.FirstName} {user.LastName}".Trim();
        }

        // دالة مساعدة للحصول على اسم الخدمة
        private string GetServiceName(Request request)
        {
            if (request.Service != null)
                return request.Service.ServiceName;
            else if (!string.IsNullOrEmpty(request.RequestType))
                return request.RequestType;
            else
                return "غير محدد";
        }

        // دالة مساعدة للحصول على حالة الطلب
        private string GetRequestStatus(Request request)
        {
            // إعطاء الأولوية لحالة الأدمن إذا كانت موجودة
            if (!string.IsNullOrEmpty(request.AdminStatus))
                return request.AdminStatus;
            else if (!string.IsNullOrEmpty(request.SupervisorStatus))
                return request.SupervisorStatus;
            else
                return "جديد";
        }

        [HttpGet]
        public IActionResult PrintReport()
        {
            return View();
        }

        // Action method للإستعلام القديم (نتركه كما هو)
        [HttpPost]
        public IActionResult GenerateCustomReport(string ReportTitle, string ReportType, DateTime FromDate, DateTime ToDate, List<string> SelectedFields)
        {
            var fakeData = new List<DeviceReportViewModel>
            {
                new DeviceReportViewModel { Id = 1, DeviceName = "طابعة ثلاثية الأبعاد - 001", DeviceType = "طابعة ثلاثية الأبعاد", Status = "تشغيل" },
                new DeviceReportViewModel { Id = 2, DeviceName = "حاسوب محمول - 002", DeviceType = "حاسبات", Status = "صيانة" },
                new DeviceReportViewModel { Id = 3, DeviceName = "جهاز قياس - 003", DeviceType = "أجهزة قياس", Status = "تشغيل" },
                new DeviceReportViewModel { Id = 4, DeviceName = "طابعة ثلاثية الأبعاد - 004", DeviceType = "طابعة ثلاثية الأبعاد", Status = "خارج الخدمة" },
                new DeviceReportViewModel { Id = 5, DeviceName = "حاسوب مكتبي - 005", DeviceType = "حاسبات", Status = "تشغيل" }
            };

            ViewBag.ReportTitle = ReportTitle;
            ViewBag.ReportType = ReportType;
            ViewBag.FromDate = FromDate.ToString("yyyy-MM-dd");
            ViewBag.ToDate = ToDate.ToString("yyyy-MM-dd");

            return View("PrintReport", fakeData);
        }
    }
}