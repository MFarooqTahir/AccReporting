using AccReporting.Server.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using System.Security.Claims;

namespace AccReporting.Server
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        private ApplicationDbContext _appliationDbContext;

        public MyUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        IOptions<IdentityOptions> optionsAccessor,
        ApplicationDbContext applicationDbContext)
            : base(userManager: userManager, optionsAccessor: optionsAccessor)
        {
            _appliationDbContext = applicationDbContext;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            //get the data from dbcontext
            //var Iuser = _appliationDbContext.Users.Where(x => x.EmailConfirmed).FirstOrDefault();

            var identity = await base.GenerateClaimsAsync(user: user);
            //Get the data from EF core
            identity.AddClaims(
           claims: new[] {
            new Claim(type: ClaimTypes.NameIdentifier, value: user.Id)
           });

            return identity;
        }
    }
}