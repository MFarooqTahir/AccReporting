using AccReporting.Server.Data;
using AccReporting.Server.Services;
using AccReporting.Shared.DTOs;

using EFCore.BulkExtensions;

using HashidsNet;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using System.Text;

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

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger, IHashids hash, DataService dataService)
        {
            _context = context;
            _logger = logger;
            _hash = hash;
            _dataService = dataService;
        }

        [AllowAnonymous]
        [HttpPost("fileupload")]
        public async Task fileupload(IList<IFormFile> UploadFiles, CancellationToken ct)
        {
            try
            {
                var id = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
                string text = string.Empty;
                var file = UploadFiles[0];
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    byte[]? fileBytes = ms.ToArray();
                    text = Encoding.Default.GetString(fileBytes);
                }
                _logger.LogInformation("File uploaded");
                await _dataService.SetDbName("Test", ct);
                var res = await _dataService.InsertAllDataBulk(text, ct);
                _logger.LogInformation("Data Inserted");
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Data Error: {Message}", ex.Message);
            }
        }

        [HttpGet("ChangeSelectedCompany")]
        public async Task<bool> ChangeSelectedCompany(string ID, CancellationToken ct)
        {
            try
            {
                var id = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
                int curr = _hash.DecodeSingle(ID);
                var res = _context.CompanyAccounts.FirstOrDefault(a => a.ID == curr && a.UserID == id);
                if (res != null)
                {
                    res.IsSelected = true;
                    var Selected = _context.CompanyAccounts.FirstOrDefault(a => a.IsSelected && a.UserID == id);
                    if (Selected != null)
                    {
                        Selected.IsSelected = false;
                    }
                    await _context.BulkSaveChangesAsync(cancellationToken: ct);
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

                var ret = await _context.CompanyAccounts.AsNoTracking()
                    .Where(x => x.UserID == id)
                    .OrderBy(x => x.CompRole)
                    .Select(x => new CompaniesListDTO(_hash.Encode(x.ID), _hash.Encode(x.Company.ID), x.Company.Name, x.AcNumber, x.CompRole, x.IsSelected))
                    .ToListAsync(ct);
                if (ret.Count == 0)
                {
                    _logger.LogInformation("No account found");
                    ret.AddRange(
                        new[]
                        {
                            new CompaniesListDTO(_hash.Encode(2),_hash.Encode(2), "No Company Name", "5.6.7", "Admin", true),
                            new CompaniesListDTO(_hash.Encode(3),_hash.Encode(3), "No Company Name", "5.73.4", "Admin", false),
                            new CompaniesListDTO(_hash.Encode(0),_hash.Encode(0), "No Company Name", "1.25", "User", false),
                            new CompaniesListDTO(_hash.Encode(1),_hash.Encode(1), "No Company Name", "1.5.3", "User", false),
                            new CompaniesListDTO(_hash.Encode(4),_hash.Encode(4), "No Company Name", "8.75", "User", false),
                        });
                }

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