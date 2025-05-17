namespace FutureVendWeb.Data.Models.PaymentDevice
{
    public class UpdatePaymentDeviceModel
    {
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string OSVersion { get; set; }
        public bool NFC { get; set; }
        public bool Chip { get; set; }
    }
}
