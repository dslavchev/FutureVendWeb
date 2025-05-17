using FutureVendWeb.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Data.Models.Device
{
    public class EditDeviceModel
    {
        public int Id { get; set; }
        public string PaymentDeviceSerial { get; set; }
        public string VendingDeviceSerial { get; set; }
        public int PaymentDeviceId { get; set; }
        public List<SelectListItem> PaymentDevices { get; set; }

        public List<SelectListItem> VendingDevices { get; set; }

        public List<SelectListItem> Customer { get; set; }

        public int VendingDeviceId { get; set; }
        public int CustomerId { get; set; }
        public bool AcceptCard { get; set; }
        public bool AcceptCash { get; set; }
        public double LocationLat { get; set; }
        public double LocationLon { get; set; }
    }
}
