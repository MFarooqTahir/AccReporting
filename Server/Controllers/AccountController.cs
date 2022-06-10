using AccReporting.Server.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace AccReporting.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<ApplicationUser> _signinManager;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> _signinManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            this._signinManager = _signinManager;
        }

        [HttpGet("GetCurrentSelectedCompany")]
        public async Task<CompanyAccount?> GetCurrentSelectedCompany(CancellationToken ct)
        {
            try
            {
                var resx = _context.CompanyAccounts.AsNoTracking();

                var id = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
                var ret = await resx.FirstOrDefaultAsync(x => x.UserID == id && x.IsSelected == true, ct);
                _logger.LogInformation("Got account");

                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error in getting user, {Message}", ex.Message);
                return null;
            }
        }
    }
}