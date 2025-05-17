using FutureVendWeb.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Data.Entities
{
    public class CustomerEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string CompanyName { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        public string Address { get; set; }
        
        public string City { get;set; }
        
        public string PostCode { get; set; }
        
        public string Country { get;  set; }

        public string Phone { get;  set; }

        [EmailAddress]
        public string Email { get;  set; }

        [Required]
        public string TaxNumber { get;  set; }

        [Required]
        public int UserId { get; set; }
    }
}
