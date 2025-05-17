using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Data.Entities
{
    public class PaymentDeviceEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Manufacturer { get; set; }

        public string OSVersion { get; set; }
        
        public bool NFC { get; set; }
        
        public bool Chip { get; set; }

        [Required]
        public int UserId { get; set; } 
    }

}
