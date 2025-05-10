using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Transactions;
using FutureVendWeb.Models;

namespace FutureVendWeb.Data.Entities
{
    public class Device
    {
        [Key]
        public int Id { get; set; }

        public string PaymentDeviceSerial { get; set; }
        public string VendingDeviceSerial { get; set; }

        public int PaymentDeviceId { get; set; }
        public PaymentDevice? PaymentDevice { get; set; }

        public int VendingDeviceId { get; set; }
        public VendingDevice ? VendingDevice { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public bool AcceptCard { get; set; }
        public bool AcceptCash { get; set; }

        public double LocationLat { get; set; }
        public double LocationLon { get; set; }


        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }


        // public ICollection<Transaction> Transactions { get; set; }
    }

}
