namespace FutureVendWeb.Data.Models.Transaction
{
    public class GetAllTransactionWithViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string DeviceInformation { get; set; }

        public string VendingProductInformation { get; set; }

        public string CustomerInformation { get; set; }
    }
}
