using FutureVendWeb.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Data.Models.Transaction
{
    public class GetTransactionModel
    {
        public int Id { get; set; }
        
        public decimal Amount { get; set; }
        
        public string Currency { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public string DeviceInformation { get; set; }

        public string VendingProductInformation { get; set; }

        public string CustomerInformation { get; set; }

        public int DeviceId { get; set; }
        
        public DeviceEntity Device { get; set; }
        public int VendingProductId { get; set; }
        
        public VendingProductEntity VendingProduct { get; set; }
        public string PaymentType { get; set; }
    }
}
