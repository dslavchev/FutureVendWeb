using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Data.Entities
{
    public class VendingDeviceEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string Manufacturer { get; set; }
        
        public string SoftwareVersion { get; set; }

        [Required]
        public int UserId { get; set; }
    }

}
