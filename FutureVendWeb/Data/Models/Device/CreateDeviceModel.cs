using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Data.Models.Device
{
    public class CreateDeviceModel
    {
        [Required]
        public string PaymentDeviceSerial { get; set; }

        [Required]

        public string VendingDeviceSerial { get; set; }

        [Required]
        public int PaymentDeviceId { get; set; }

        [Required]
        public int VendingDeviceId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        public bool AcceptCard { get; set; }
        public bool AcceptCash { get; set; }

        [Range(-90, 90)]
        public double LocationLat { get; set; }

        [Range(-180, 180)]
        public double LocationLon { get; set; }
        public List<SelectListItem> PaymentDevices { get; set; }
        public List<SelectListItem> VendingDevices { get; set; }
        public List<SelectListItem> Customers { get; set; }
    }
}
