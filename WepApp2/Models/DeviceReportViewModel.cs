namespace WepApp2.Models
{
    public class DeviceReportViewModel
    {
        public int Id { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string Location { get; set; }
        public string Company { get; set; }
        public string Model { get; set; }
        public string Status { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
    }
}