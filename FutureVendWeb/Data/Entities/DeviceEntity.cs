using System.ComponentModel.DataAnnotations;
using FutureVendWeb.Data.Models;

namespace FutureVendWeb.Data.Entities
{
    public class DeviceEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PaymentDeviceSerial { get; set; }
        
        [Required]
        public string VendingDeviceSerial { get; set; }
        
        [Required]
        public int PaymentDeviceId { get; set; }
        
        public PaymentDeviceEntity? PaymentDevice { get; set; }
        
        [Required]
        public int VendingDeviceId { get; set; }
    
        public VendingDeviceEntity ? VendingDevice { get; set; }

        [Required]
        public int CustomerId { get; set; }
        
        public CustomerEntity? Customer { get; set; }
        
        public bool AcceptCard { get; set; }
        
        public bool AcceptCash { get; set; }

        [Range(-90, 90, ErrorMessage = "Ширината трябва да е между -90 и 90.")]
        public double LocationLat { get; set; }
        
        [Range(-180, 180, ErrorMessage = "Дължината трябва да е между -180 и 180.")]
        public double LocationLon { get; set; }

        [Required]
        public int UserId { get; set; }
    }

}
