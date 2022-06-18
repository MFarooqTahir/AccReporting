using Microsoft.AspNetCore.Identity;

namespace AccReporting.Shared.ContextModels
{
    public class ApplicationUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public ICollection<CompanyAccount>? Companies { get; set; }
    }
}