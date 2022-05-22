using Microsoft.AspNetCore.Identity;

namespace AccReporting.Shared.ContextModels
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<CompanyAccount>? Companies { get; set; }
    }
}