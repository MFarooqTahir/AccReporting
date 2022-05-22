using AccReporting.Server.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using System.Security.Claims;

namespace AccReporting.Server
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser>
    {
        private ApplicationDbContext _appliationDbContext;

        public MyUserClaimsPrincipalFactory(
        UserManager<IdentityUser> userManager,
        IOptions<IdentityOptions> optionsAccessor,
        ApplicationDbContext applicationDbContext)
            : base(userManager, optionsAccessor)
        {
            _appliationDbContext = applicationDbContext;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            //get the data from dbcontext
            //var Iuser = _appliationDbContext.Users.Where(x => x.EmailConfirmed).FirstOrDefault();

            var identity = await base.GenerateClaimsAsync(user);
            //Get the data from EF core

            //identity.AddClaim(new Claim(Claim, Iuser.Email));
            return identity;
        }
    }
}