using AccReporting.Server.Data;
using AccReporting.Server.Services;
using AccReporting.Shared.DTOs;

using HashidsNet;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace AccReporting.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IHashids _hash;
        private readonly DataService _dataService;
        private readonly Regex removeSpaces = new Regex(@"\s+", RegexOptions.Compiled);

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger, IHashids hash, DataService dataService)
        {
            _context = context;
            _logger = logger;
            _hash = hash;
            _dataService = dataService;
        }

        [HttpPost("AddOrUpdateAccount")]
        public async Task<bool> AddOrUpdateAccount(DisplayUsersDto model, CancellationToken ct)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var adminCompany = await _context.CompanyAccounts.AsNoTracking().Where(x => x.Company.Approved && x.IsSelected && x.UserID == userId && x.CompRole == "Admin")
                    .Select(x => x.Company)
                    .FirstAsync(ct);
                var user = await _context.Users.AsNoTracking().FirstAsync(x => x.Email == model.UserEmail, ct);
                var exists = await _context.CompanyAccounts.FirstOrDefaultAsync(x => x.CompanyID == adminCompany.ID && x.UserID == user.Id, ct);
                if (exists is null || exists.ID == 0)
                {
                    var newAccount = new CompanyAccount()
                    {
                        Company = adminCompany,
                        CompRole = "User",
                        User = user,
                        AcNumber = model.AcCode
                    };
                    await _context.AddAsync(newAccount, ct);
                }
                else
                {
                    exists.AcNumber = model.AcCode;
                }
                await _context.SaveChangesAsync(cancellationToken: ct);
                _logger.LogInformation("Saved Account");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Data Error: {Message}", ex.Message);
                return false;
            }
        }

        [HttpGet("GetUserAccounts")]
        public async Task<IEnumerable<DisplayUsersDto>> GetUserAccounts(CancellationToken ct)
        {
            try
            {
                int c = 1;
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var data = _context.CompanyAccounts.Where(x => x.Company.Approved && x.UserID == userId && x.IsSelected)
                    .Select(y => new { y.AcNumber, y.Company.DbName }).First();
                await _dataService.SetDbName(data.DbName, ct);
                var list = (await _dataService.GetAcFileAsync(ct)).Select(x => x.ActCode).Distinct();

                var accountID = await _context.CompanyAccounts.Where(x => x.Company.Approved && x.IsSelected && x.UserID == userId && x.CompRole == "Admin")
                    .Select(x => x.CompanyID).FirstAsync();
                var ret = await _context.Companies.AsNoTracking()
                    .Where(x => x.ID == accountID)
                    .Include(x => x.CompanyUsers)
                    .ThenInclude(x => x.User)
                    .FirstAsync(cancellationToken: ct);
                var alUsers = ret.CompanyUsers.Select(x => x.AcNumber).ToList();

                var accounts = ret.CompanyUsers.Select(x => new DisplayUsersDto()
                {
                    ID = c++,
                    AcCode = x.AcNumber,
                    UserEmail = x.User.Email,
                    IsAdmin = x.CompRole == "Admin",
                }).ToList();
                (accounts ??= new()).AddRange(list.Where(x => !alUsers.Contains(x)).Select(x => new DisplayUsersDto()
                {
                    ID = c++,
                    AcCode = x,
                    UserEmail = "",
                    IsAdmin = false,
                }));
                _logger.LogInformation("Got Accounts");
                return accounts;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Data Error: {Message}", ex.Message);
                return new List<DisplayUsersDto>();
            }
        }

        [HttpPost("CreateCompany")]
        public async Task<bool> CreateCompany(NewCompanyInputDto inp, CancellationToken ct)
        {
            try
            {
                var exists = await _context.Companies.AnyAsync(x => x.Name == inp.Name, ct);
                if (!exists)
                {
                    _logger.LogInformation("Account doesnt exist, adding");
                    var id = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
                    var user = await _context.Users.FirstAsync(x => x.Id == id);
                    var newCompany = new CompanyDetail()
                    {
                        Name = inp.Name,
                        Phone = inp.phone,
                        Address = inp.Address,
                        DbName = inp.Name.Length < 30 ? removeSpaces.Replace(inp.Name, "_") : removeSpaces.Replace(inp.Name[..30], "_"),
                        Approved = true,
                    };
                    await _context.AddAsync(newCompany, ct);
                    var compAccount = new CompanyAccount()
                    {
                        CompRole = "Admin",
                        User = user,
                        Company = newCompany,
                        IsSelected = false,
                    };

                    await _context.AddAsync(compAccount, ct);

                    //var allSelected = await _context.CompanyAccounts.Where(x => x.Company.Approved && x.UserID == id && x.IsSelected).ToListAsync(ct);
                    //allSelected.ForEach(x => x.IsSelected = false);

                    await _context.SaveChangesAsync(cancellationToken: ct);
                    _logger.LogInformation("Account added");

                    return true;
                }
                _logger.LogInformation("Account exist, not adding");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error: {Message}", ex.Message);
                return false;
            }
        }

        [AllowAnonymous]
        [HttpPost("fileupload")]
        public async Task<IActionResult> fileupload(CancellationToken ct)
        {
            bool res;
            try
            {
                var id = Request.Headers.First(x => string.Equals(x.Key, "ID", StringComparison.InvariantCultureIgnoreCase)).Value[0];
                string text = string.Empty;
                var file = Request.Form.Files[0];
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    byte[]? fileBytes = ms.ToArray();
                    text = Encoding.Default.GetString(fileBytes);
                }
                _logger.LogInformation("File uploaded");
                var dbname = _context.CompanyAccounts.AsNoTracking()
                    .Where(x => x.Company.Approved && x.UserID == id && x.CompRole == "Admin" && x.IsSelected)
                    .Select(x => x.Company.DbName)
                    .First();
                await _dataService.SetDbName(dbname, ct);
                res = await _dataService.InsertAllDataBulk(text, ct);
                _logger.LogInformation("Data Inserted");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Data Error: {Message}", ex.Message);
                return NotFound();
            }
            return Ok(res);
        }

        [HttpGet("ChangeSelectedCompany")]
        public async Task<bool> ChangeSelectedCompany(string ID, CancellationToken ct)
        {
            try
            {
                var id = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
                int curr = _hash.DecodeSingle(ID);
                var res = _context.CompanyAccounts.FirstOrDefault(a => a.ID == curr && a.UserID == id && a.Company.Approved);
                if (res != null)
                {
                    var Selected = await _context.CompanyAccounts.Where(a => a.IsSelected && a.UserID == id).ToListAsync(ct);
                    Selected?.ForEach(x => x.IsSelected = false);
                    res.IsSelected = true;
                    await _context.SaveChangesAsync(cancellationToken: ct);
                    _logger.LogInformation("Updated Selected Company");
                    return true;
                }
                _logger.LogInformation("Company not found");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error occured, {Message}", ex.Message);
                return false;
            }
        }

        [HttpGet("GetAllCompaniesForUser")]
        public async Task<IEnumerable<CompaniesListDTO?>?> GetCurrentSelectedCompany(CancellationToken ct)
        {
            try
            {
                var id = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
                var retx = await _context.CompanyAccounts.AsNoTracking()
                    .Where(x => x.Company.Approved && x.UserID == id)
                    .OrderBy(x => x.CompRole)
                    .Select(x => new { x.ID, CompID = x.Company.ID, x.Company.Name, x.AcNumber, x.CompRole, x.IsSelected })
                    .ToListAsync(ct);
                var ret = retx
                    .Select(x =>
                    new CompaniesListDTO(_hash.Encode(x.ID), _hash.Encode(x.CompID), x.Name, x.AcNumber, x.CompRole, x.IsSelected));
                //if (!ret.Any())
                //{
                //    _logger.LogInformation("No account found");
                //    ret = new List<CompaniesListDTO>(
                //        new[]
                //        {
                //            new CompaniesListDTO(_hash.Encode(2),_hash.Encode(2), "No Company Name", "5.6.7", "Admin", true),
                //            new CompaniesListDTO(_hash.Encode(3),_hash.Encode(3), "No Company Name", "5.73.4", "Admin", false),
                //            new CompaniesListDTO(_hash.Encode(0),_hash.Encode(0), "No Company Name", "1.25", "User", false),
                //            new CompaniesListDTO(_hash.Encode(1),_hash.Encode(1), "No Company Name", "1.5.3", "User", false),
                //            new CompaniesListDTO(_hash.Encode(4),_hash.Encode(4), "No Company Name", "8.75", "User", false),
                //        });
                //}

                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error in getting user, {Message}", ex.Message);
                return new List<CompaniesListDTO>();
            }
        }
    }
}