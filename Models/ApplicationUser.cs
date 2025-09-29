using Microsoft.AspNetCore.Identity;

namespace Fiesta_Flavors.Models
{
    public class ApplicationUser:IdentityUser
    {
        public ICollection<Order>? Orders { get; set; }
    }
}
