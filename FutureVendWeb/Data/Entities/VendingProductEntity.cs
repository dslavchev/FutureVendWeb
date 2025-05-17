using System.ComponentModel.DataAnnotations;

namespace FutureVendWeb.Data.Entities
{
    public class VendingProductEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PLU { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public string Category { get; set; }
        
        [Required]
        public int UserId { get; set; }
    }

}
