using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Data.Entities
{
    public class TransactionEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(0.01, 10000)]
        public decimal Amount { get; set; }
        
        [Required]
        [StringLength(3)]
        public string Currency { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int DeviceId { get; set; }

        public DeviceEntity? Device { get; set; }

        [Required]
        public int VendingProductId { get; set; }

        public VendingProductEntity? VendingProduct { get; set; }

        [Required]
        [RegularExpression("cash|card", ErrorMessage = "PaymentType must be 'cash' or 'card'")]
        public string PaymentType { get; set; }
    }

}