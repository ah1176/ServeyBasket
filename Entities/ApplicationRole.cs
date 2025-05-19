using Microsoft.AspNetCore.Identity;

namespace Survey_Basket.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
    }
}
