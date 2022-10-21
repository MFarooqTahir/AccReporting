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
    [Route(template: "api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly IHashids _hash;
        private readonly DataService _dataService;
        private readonly Regex _removeSpaces = new(pattern: @"\s+", options: RegexOptions.Compiled);

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger, IHashids hash, DataService dataService)
        {
            _context = context;
            _logger = logger;
            _hash = hash;
            _dataService = dataService;
        }

        [HttpPost(template: nameof(AddOrUpdateAccount))]
        public async Task<bool> AddOrUpdateAccount(DisplayUsersDto model, CancellationToken ct)
        {
            try
            {
                var userId = User.FindFirstValue(claimType: ClaimTypes.NameIdentifier);
                var adminCompany = await _context.CompanyAccounts.Where(predicate: x => x.Company!.Approved && x.IsSelected && x.UserId == userId && x.CompRole == "Admin")
                    .Select(selector: x => x.Company)
                    .SingleAsync(cancellationToken: ct);
                if (adminCompany is null)
                {
                    _logger.LogError("Error: Admin company not found");
                    return false;
                }
                var user = await _context.Users.FirstAsync(predicate: x => x.Email == model.UserEmail, cancellationToken: ct);
                if (user is null)
                {
                    _logger.LogError("Error: user account not found");
                    return false;
                }
                var exists = await _context.CompanyAccounts.FirstOrDefaultAsync(predicate: x => x.CompanyId == adminCompany!.Id && x.UserId == user.Id, cancellationToken: ct);
                if (exists is null || exists.Id == 0)
                {
                    var newAccount = new CompanyAccount()
                    {
                        CompRole = "User",
                        AcNumber = model.AcCode
                    };
                    user!.Companies ??= new List<CompanyAccount>();
                    user.Companies.Add(newAccount);
                    adminCompany!.CompanyUsers ??= new List<CompanyAccount>();
                    adminCompany.CompanyUsers.Add(newAccount);
                    //await _context.AddAsync(entity: newAccount, cancellationToken: ct);
                }
                else
                {
                    exists.AcNumber = model.AcCode;
                }
                await _context.SaveChangesAsync(cancellationToken: ct);
                _logger.LogInformation(message: "Saved Account");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(message: "Data Error: {Message}", ex.Message);
                return false;
            }
        }

        [HttpGet(template: nameof(GetUserAccounts))]
        public async Task<IEnumerable<DisplayUsersDto>> GetUserAccounts(CancellationToken ct)
        {
            try
            {
                var c = 1;
                var userId = User.FindFirstValue(claimType: ClaimTypes.NameIdentifier);
                var data = await _context.CompanyAccounts.Where(predicate: x => x.Company!.Approved && x.UserId == userId && x.IsSelected)
                    .Select(selector: y => new { y.AcNumber, y.Company!.DbName }).SingleAsync(cancellationToken: ct);
                await _dataService.SetDbName(dbName: data.DbName!, ct: ct);
                var list = await (await _dataService.GetDbContext(ct: ct)).Acfiles.Select(selector: x => x.ActCode).Distinct().ToArrayAsync(cancellationToken: ct);

                var accountId = await _context.CompanyAccounts.Where(predicate: x => x.Company!.Approved && x.IsSelected && x.UserId == userId && x.CompRole == "Admin")
                    .Select(selector: x => x.CompanyId).FirstAsync();
                var ret = await _context.Companies.AsNoTracking()
                    .Where(predicate: x => x.Id == accountId)
                    .Include(navigationPropertyPath: x => x.CompanyUsers)!
                    .ThenInclude(navigationPropertyPath: x => x.User)
                    .FirstAsync(cancellationToken: ct);
                if (ret.CompanyUsers is not null)
                {
                    var alUsers = ret.CompanyUsers.Select(selector: x => x.AcNumber).ToList();
                    var accounts = ret.CompanyUsers.Select(selector: x => new DisplayUsersDto()
                    {
                        Id = c++,
                        AcCode = x.AcNumber ?? "",
                        UserEmail = x.User?.Email ?? "",
                        IsAdmin = x.CompRole == "Admin",
                    }).ToList();

                    accounts ??= new List<DisplayUsersDto>();
                    accounts.AddRange(collection: list.Where(predicate: x => !alUsers.Contains(item: x)).Select(selector: x =>

                        new DisplayUsersDto()
                        {
                            Id = c++,
                            AcCode = x,
                            UserEmail = "",
                            IsAdmin = false,
                        }
                    ));

                    _logger.LogInformation(message: "Got Accounts");
                    return accounts;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation(message: "Data Error: {Message}", ex.Message);
                return new List<DisplayUsersDto>();
            }

            return Array.Empty<DisplayUsersDto>();
        }

        [HttpPost(template: nameof(CreateCompany))]
        public async Task<bool> CreateCompany(NewCompanyInputDto inp, CancellationToken ct)
        {
            try
            {
                var exists = await _context.Companies.AnyAsync(predicate: x => x.Name == inp.Name, cancellationToken: ct);
                if (!exists)
                {
                    _logger.LogInformation(message: "Account doesnt exist, adding");
                    var id = User.Claims.First(predicate: a => a.Type == ClaimTypes.NameIdentifier).Value;
                    var user = await _context.Users.SingleAsync(predicate: x => x.Id == id, cancellationToken: ct);
                    var newCompany = new CompanyDetail()
                    {
                        Name = inp.Name,
                        Phone = inp.Phone,
                        Address = inp.Address,
                        DbName = inp.Name.Length < 30 ? _removeSpaces.Replace(input: inp.Name.Trim(), replacement: "_") : _removeSpaces.Replace(input: inp.Name.Trim()[..30], replacement: "_"),
                        Approved = true,
                    };
                    await _context.AddAsync(entity: newCompany, cancellationToken: ct);
                    var compAccount = new CompanyAccount()
                    {
                        CompRole = "Admin",
                        User = user,
                        Company = newCompany,
                        IsSelected = false,
                    };

                    await _context.AddAsync(entity: compAccount, cancellationToken: ct);
                    await _context.SaveChangesAsync(cancellationToken: ct);
                    _logger.LogInformation(message: "Account added");

                    return true;
                }
                _logger.LogInformation(message: "Account exist, not adding");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(message: "Error: {Message}", ex.Message);
                return false;
            }
        }

        [AllowAnonymous]
        [HttpPost(template: "fileupload")]
        public async Task<IResult> Fileupload(CancellationToken ct)
        {
            bool res;
            try
            {
                var id = Request.Headers.First(predicate: x => string.Equals(a: x.Key, b: "ID", comparisonType: StringComparison.InvariantCultureIgnoreCase)).Value[index: 0];
                var text = string.Empty;
                var file = Request.Form.Files[index: 0];
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(target: ms);
                    var fileBytes = ms.ToArray();
                    text = Encoding.Default.GetString(bytes: fileBytes);
                }
                _logger.LogInformation(message: "File uploaded");
                var dbNameSingleAsync = await _context.CompanyAccounts.AsNoTracking()
                    .Where(predicate: x => x.Company!.Approved && x.UserId == id && x.CompRole == "Admin" && x.IsSelected)
                    .Select(selector: x => x.Company!.DbName)
                    .SingleAsync(cancellationToken: ct) ?? "";
                await _dataService.SetDbName(dbName: dbNameSingleAsync, ct: ct);
                res = await _dataService.InsertAllDataBulk(access: text, ct: ct);
                _logger.LogInformation(message: "Data Inserted");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(message: "Data Error: {Message}", ex.Message);
                return Results.NoContent();
            }
            return res ? Results.Ok() : Results.NoContent();
        }

        [HttpGet(template: nameof(ChangeSelectedCompany))]
        public async Task<bool> ChangeSelectedCompany(string id, CancellationToken ct)
        {
            try
            {
                var s = User.Claims.First(predicate: a => a.Type == ClaimTypes.NameIdentifier).Value;
                var curr = _hash.DecodeSingle(hash: id);
                var res = await _context.CompanyAccounts.SingleOrDefaultAsync(predicate: a => a.Id == curr && a.UserId == s && a.Company!.Approved, cancellationToken: ct);
                if (res is not null)
                {
                    var selected = await _context.CompanyAccounts.Where(predicate: a => a.IsSelected && a.UserId == s).ToListAsync(cancellationToken: ct);
                    selected?.ForEach(action: x => x.IsSelected = false);
                    res.IsSelected = true;
                    await _context.SaveChangesAsync(cancellationToken: ct);
                    _logger.LogInformation(message: "Updated Selected Company");
                    return true;
                }
                _logger.LogInformation(message: "Company not found");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogInformation(message: "Error occured, {Message}", ex.Message);
                return false;
            }
        }

        [HttpGet(template: "GetAllCompaniesForUser")]
        public async Task<IEnumerable<CompaniesListDto?>?> GetCurrentSelectedCompany(CancellationToken ct)
        {
            try
            {
                var id = User.Claims.First(predicate: a => a.Type == ClaimTypes.NameIdentifier).Value;
                var retx = await _context.CompanyAccounts.AsNoTracking()
                    .Where(predicate: x => x.Company!.Approved && x.UserId == id)
                    .OrderBy(keySelector: x => x.CompRole)
                    .Select(selector: x => new { ID = x.Id, CompID = x.Company!.Id, x.Company.Name, x.AcNumber, x.CompRole, x.IsSelected })
                    .ToListAsync(cancellationToken: ct);
                return retx
                    .Select(selector: x =>
                    new CompaniesListDto(id: _hash.Encode(number: x.ID), compId: _hash.Encode(number: x.CompID), name: x.Name!, accountNo: x.AcNumber, role: x.CompRole!, isSelected: x.IsSelected));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(message: "Error in getting user, {Message} {Stack}", ex.Message, ex.StackTrace);
                return new List<CompaniesListDto>();
            }
        }
    }
}