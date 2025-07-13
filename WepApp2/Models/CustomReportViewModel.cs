namespace WepApp2.Models
{
    public class CustomReportViewModel
    {
        public string ReportTitle { get; set; }
        public string ReportType { get; set; }
        public string ServiceType { get; set; }
        public string DeviceStatus { get; set; }  // إضافة خاصية حالة الجهاز
        public string UserType { get; set; }       // إضافة خاصية نوع المستخدم
        public string RequestStatus { get; set; }  // إضافة خاصية حالة الطلب
        public List<string> Fields { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}