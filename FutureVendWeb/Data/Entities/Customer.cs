using FutureVendWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Data.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get;set; }
        public string PostCode { get; set; }
        public string Country { get;  set; }
        public string Phone { get;  set; }
        public string Email { get;  set; }
        public string TaxNumber { get;  set; }
        
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // public ICollection<Device> Devices { get;  set;}
        //  public ICollection<CustomerVendingProduct> CustomerVendingProducts { get;  set; }
    }
}
