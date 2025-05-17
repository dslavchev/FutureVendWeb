namespace FutureVendWeb.Data.Models.Device
{
    public class GetAllDevicesWithViewModel
    {
        public int Id { get; set; }
        public string PaymentDeviceSerial { get; set; }
        public string VendingDeviceSerial { get; set; }

        public string PaymentDeviceInformation { get; set; }

        public string VendingDeviceInformation { get; set; }

        public string CustomerInformation { get; set; }

        public int VendingDeviceId { get; set; }
        public int CustomerId { get; set; }
    }
}
