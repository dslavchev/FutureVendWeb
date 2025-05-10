using Microsoft.AspNetCore.Identity;

namespace FutureVendWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } 
    }
}
