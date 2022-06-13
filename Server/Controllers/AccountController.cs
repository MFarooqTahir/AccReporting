﻿using AccReporting.Server.Data;
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

        [HttpPost("CreateCompany"), HttpGet("CreateCompany")]
        public async Task<bool> CreateCompany(string name, string phone, string address, CancellationToken ct)
        {
            try
            {
                var exists = await _context.Companies.AnyAsync(x => x.Name == name, ct);
                if (!exists)
                {
                    _logger.LogInformation("Account doesnt exist, adding");
                    var id = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
                    var user = await _context.Users.FirstAsync(x => x.Id == id);
                    var newCompany = new CompanyDetail()
                    {
                        Name = name,
                        Phone = phone,
                        Address = address,
                        DbName = name.Length < 15 ? name : name[..15],
                    };
                    await _context.AddAsync(newCompany, ct);
                    var compAccount = new CompanyAccount()
                    {
                        CompRole = "Admin",
                        User = user,
                        Company = newCompany,
                        IsSelected = true,
                    };

                    await _context.AddAsync(compAccount, ct);
                    var updateCols = new List<string> { nameof(CompanyAccount.IsSelected) };
                    await _context.CompanyAccounts.BatchUpdateAsync(new CompanyAccount() { IsSelected = false }, updateCols, cancellationToken: ct);

                    await _context.BulkSaveChangesAsync(cancellationToken: ct);
                    _logger.LogInformation("Account added");

                    return true;
                }
                _logger.LogInformation("Account exist, not adding");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Data Error: {Message}", ex.Message);
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
                    .Where(x => x.UserID == id && x.CompRole == "Admin" && x.IsSelected)
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
                var res = _context.CompanyAccounts.FirstOrDefault(a => a.ID == curr && a.UserID == id);
                if (res != null)
                {
                    var Selected = await _context.CompanyAccounts.Where(a => a.IsSelected && a.UserID == id).ToListAsync(ct);
                    Selected?.ForEach(x => x.IsSelected = false);
                    res.IsSelected = true;
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
                var retx = await _context.CompanyAccounts.AsNoTracking()
                    .Where(x => x.UserID == id)
                    .OrderBy(x => x.CompRole)
                    .Select(x => new { x.ID, CompID = x.Company.ID, x.Company.Name, x.AcNumber, x.CompRole, x.IsSelected })
                    .ToListAsync(ct);
                var ret = retx
                    .Select(x =>
                    new CompaniesListDTO(_hash.Encode(x.ID), _hash.Encode(x.CompID), x.Name, x.AcNumber, x.CompRole, x.IsSelected));
                if (!ret.Any())
                {
                    _logger.LogInformation("No account found");
                    ret = new List<CompaniesListDTO>(
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