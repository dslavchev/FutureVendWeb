using FutureVendWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Data.Entities
{
    public class PaymentDevice
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string OSVersion { get; set; }
        public bool NFC { get; set; }
        public bool Chip { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }


        //public ICollection<Device> Devices { get; set; } // навигационно свойство
    }

}
