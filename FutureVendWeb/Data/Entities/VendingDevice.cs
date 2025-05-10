using FutureVendWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Data.Entities
{
    public class VendingDevice
    {
        [Key]
        public int Id { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string SoftwareVersion { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // public ICollection<Device> Devices { get; set; }
    }

}
