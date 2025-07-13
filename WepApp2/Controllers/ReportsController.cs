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
        public IActionResult CreateCustomReport(string reportTitle, string reportType, DateTime? fromDate, DateTime? toDate, string requestStatus, string deviceStatus, string userType, string serviceType, List<string> fields)
        {
            try
            {
                // إضافة معلومات إضافية للـ ViewBag
                ViewBag.ServiceType = serviceType;

                if (reportType == "تقرير الطلبات")
                {
                    // جلب البيانات من قاعدة البيانات
                    var requestsQuery = _context.Requests
                        .Include(r => r.User)
                        .Include(r => r.Service)
                        .Include(r => r.Device)
                        .AsQueryable();

                    // تطبيق فلتر التاريخ إذا تم تحديده
                    if (fromDate.HasValue)
                    {
                        requestsQuery = requestsQuery.Where(r => r.RequestDate >= fromDate.Value);
                    }
                    if (toDate.HasValue)
                    {
                        requestsQuery = requestsQuery.Where(r => r.RequestDate <= toDate.Value);
                    }

                    // تطبيق فلتر حالة الطلب إذا تم تحديده
                    if (!string.IsNullOrEmpty(requestStatus))
                    {
                        requestsQuery = requestsQuery.Where(r =>
                            r.AdminStatus == requestStatus ||
                            r.SupervisorStatus == requestStatus);
                    }

                    var requests = requestsQuery.ToList();

                    // جلب جميع المشرفين مرة واحدة لتحسين الأداء
                    var supervisorIds = requests.Select(r => r.SupervisorAssigned).Distinct().ToList();
                    var supervisors = _context.Users
                        .Where(u => supervisorIds.Contains(u.UserId))
                        .ToDictionary(u => u.UserId, u => u.FirstName + " " + u.LastName);

                    // تحويل البيانات إلى كائن ديناميكي للعرض
                    var reportData = requests.Select(r => new
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

                    ViewBag.ReportTitle = reportTitle;
                    ViewBag.ReportType = reportType;
                    ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                    ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
                    ViewBag.SelectedFields = fields ?? new List<string>();

                    return View("PrintReport", reportData);
                }
                else if (reportType == "تقرير الأجهزة")
                {
                    // جلب بيانات الأجهزة من قاعدة البيانات
                    var devicesQuery = _context.Devices.AsQueryable();

                    // تطبيق فلتر حالة الجهاز إذا تم تحديده
                    if (!string.IsNullOrEmpty(deviceStatus))
                    {
                        devicesQuery = devicesQuery.Where(d => d.DeviceStatus == deviceStatus);
                    }

                    var devices = devicesQuery.ToList();

                    // تحويل البيانات إلى كائن ديناميكي - استخدام أسماء عربية لتتوافق مع PrintReport.cshtml
                    var deviceData = devices.Select(d => new
                    {
                        Id = d.DeviceId,
                        اسم_الجهاز = d.DeviceName,
                        النوع = d.DeviceType,
                        الموقع = d.DeviceLocation ?? "غير محدد",
                        الشركة = d.BrandName ?? "غير محدد",
                        الطراز = d.DeviceModel ?? "غير محدد",
                        تاريخ_آخر_صيانة = d.LastMaintenance?.ToString("yyyy-MM-dd") ?? "غير محدد",
                        الحالة = d.DeviceStatus
                    }).ToList();

                    ViewBag.ReportTitle = reportTitle;
                    ViewBag.ReportType = reportType;
                    ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                    ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
                    ViewBag.SelectedFields = fields ?? new List<string>();

                    return View("PrintReport", deviceData);
                }
                else if (reportType == "تقرير المستخدمين")
                {
                    // جلب بيانات المستخدمين من قاعدة البيانات
                    var usersQuery = _context.Users.AsQueryable();

                    // تطبيق فلتر نوع المستخدم إذا تم تحديده
                    if (!string.IsNullOrEmpty(userType))
                    {
                        usersQuery = usersQuery.Where(u => u.UserRole == userType);
                    }

                    var users = usersQuery.ToList();

                    // تحويل البيانات إلى كائن ديناميكي مع أسماء عربية
                    var userData = users.Select(u => new
                    {
                        Id = u.UserId,
                        الاسم = u.FirstName + " " + u.LastName,
                        اسم_المستخدم = u.UserName,
                        نوع_المستخدم = u.UserRole,
                        الجهة = u.Faculty ?? "غير محدد",
                        القسم = u.Department ?? "غير محدد",
                        البريد_الإلكتروني = u.Email,
                        رقم_الجوال = u.PhoneNumber
                    }).ToList();

                    ViewBag.ReportTitle = reportTitle;
                    ViewBag.ReportType = reportType;
                    ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                    ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
                    ViewBag.SelectedFields = fields ?? new List<string>();

                    return View("PrintReport", userData);
                }
                else if (reportType == "تقرير الخدمات")
                {
                    // تقرير الخدمات بناءً على نوع الخدمة المحدد
                    var servicesQuery = _context.Requests
                        .Include(r => r.User)
                        .Include(r => r.Service)
                        .Include(r => r.Device)
                        .AsQueryable();

                    // تطبيق فلتر نوع الخدمة
                    if (!string.IsNullOrEmpty(serviceType))
                    {
                        servicesQuery = servicesQuery.Where(r => r.RequestType == serviceType ||
                            (r.Service != null && r.Service.ServiceName == serviceType));
                    }

                    // تطبيق فلتر التاريخ
                    if (fromDate.HasValue)
                    {
                        servicesQuery = servicesQuery.Where(r => r.RequestDate >= fromDate.Value);
                    }
                    if (toDate.HasValue)
                    {
                        servicesQuery = servicesQuery.Where(r => r.RequestDate <= toDate.Value);
                    }

                    var services = servicesQuery.ToList();

                    // تحويل البيانات حسب نوع الخدمة
                    var serviceData = services.Select(r => new
                    {
                        نوع_الخدمة = GetServiceName(r),
                        وصف_الخدمة = r.RequestType ?? "غير محدد",
                        تاريخ_الطلب = r.RequestDate.ToString("yyyy-MM-dd"),
                        المستخدم = GetUserFullName(r.User),
                        الحالة = GetRequestStatus(r)
                    }).ToList();

                    ViewBag.ReportTitle = reportTitle;
                    ViewBag.ReportType = reportType;
                    ViewBag.ServiceType = serviceType;
                    ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                    ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
                    ViewBag.SelectedFields = fields ?? new List<string>();

                    return View("PrintReport", serviceData);
                }
                else
                {
                    // للتقارير الأخرى
                    ViewBag.ReportTitle = reportTitle;
                    ViewBag.ReportType = reportType;
                    ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                    ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

                    return View("PrintReport", new List<object>());
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
    }
}