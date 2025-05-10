using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Models
{
    public class TransactionRequestModel
    {
        [Required]
        [Range(0.01, 10000)]
        public decimal Amount { get; set; }

        [Required]
        [RegularExpression("BGN|EUR")]
        public string Currency { get; set; }

        [Required]
        [RegularExpression("cash|card")]
        public string PaymentType { get; set; }

        [Required]
        public string ItemNumber { get; set; }

        [Required]
        public string SerialNumber { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
