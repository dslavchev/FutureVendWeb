using FutureVendWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Data.Entities
{
    public class VendingProduct
    {
        [Key]
        public int Id { get; set; }
        public string PLU { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }

        public ICollection<CustomerVendingProduct>? CustomerVendingProducts { get; set; }
    }

}
