using Microsoft.AspNetCore.Mvc.Rendering;

namespace FutureVendWeb.Data.Models.Device
{
    public class DeviceDetailsModel
    {
        public int Id { get; set; }
        public string PaymentDeviceSerial { get; set; }
        public string VendingDeviceSerial { get; set; }
        public int PaymentDeviceId { get; set; }
        public String PaymentDeviceInformation {  get; set; }
        public String VendingDeviceInformation { get; set; }
        public String CustomerInformation { get; set; }
        public int VendingDeviceId { get; set; }
        public int CustomerId { get; set; }
        public bool AcceptCard { get; set; }
        public bool AcceptCash { get; set; }
        public double LocationLat { get; set; }
        public double LocationLon { get; set; }
    }
}
