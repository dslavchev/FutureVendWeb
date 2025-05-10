using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Data.Entities
{
    public class CustomerVendingProduct
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        public int VendingProductId { get; set; }
        [ForeignKey("VendingProductId")]
        public VendingProduct VendingProduct { get; set; }
    }

}
