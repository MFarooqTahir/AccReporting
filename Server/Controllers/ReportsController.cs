using AccReporting.Server.Data;
using AccReporting.Server.Reports;
using AccReporting.Server.Services;
using AccReporting.Shared.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuestPDF.Fluent;

using System.Security.Claims;

using Throw;

namespace AccReporting.Server.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _authDb;
        private readonly ILogger<ReportsController> _logger;
        private readonly DataService _dataService;

        private readonly IDictionary<string, string> types = new Dictionary<string, string>()
                {
                    {"Sale","S" },{"Estimate","E" },{"Purchase","P" },{"Return","R"}
                };

        public ReportsController(DataService dataService, ILogger<ReportsController> logger, ApplicationDbContext authDb)
        {
            _dataService = dataService;
            _logger = logger;
            _authDb = authDb;
        }

        [HttpGet("InvSummaryList")]
        public async Task<IEnumerable<InvSummGridModel>> InvSummaryListPaged(CancellationToken ct, int page = 0, int pageSize = 0)
        {
            try
            {
                _ = Request;
                var id = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
                var data = _authDb.CompanyAccounts.Where(x => x.UserID == id && x.IsSelected)
                    .Select(y => new { y.AcNumber, y.Company.DbName }).First();
                await _dataService.SetDbName(data.DbName, ct);
                var ret = await _dataService.GetInvSummGridAsync(data.AcNumber, page, pageSize, ct);
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("There was an error {Message}", ex.Message);
                return null;
            }
        }

        [HttpGet("[action]")]
        public async Task<byte[]> SalesReport(int invNo, string type, CancellationToken ct)
        {
            try
            {
                _ = Request;
                string InpType = types[type];
                invNo.Throw()
                            .IfNegative()
                            .IfDefault();
                type.Throw()
                    .IfNullOrWhiteSpace(x => x);
                var id = User.Claims.First(a => a.Type == ClaimTypes.NameIdentifier).Value;
                var data = _authDb.CompanyAccounts
                    .Where(x => x.UserID == id && x.IsSelected)
                    .Select(y => new { y.AcNumber, y.Company.DbName, y.Company.Name, y.Company.Phone, y.Company.Address })
                    .First();
                _logger.LogInformation("Getting sales report for invoice {invNo}", invNo);
                await _dataService.SetDbName(data.DbName, ct);
                var res = await _dataService.GetSalesInvoiceData(invNo, InpType, data.AcNumber, ct);
                if (res?.InvNo != invNo)
                {
                    _logger.LogInformation("Invoice not found {invNo}", invNo);
                    return null;
                }
                res.Type = type;
                res.CompanyName = data.Name;
                res.cell = data.Phone;
                res.Address = data.Address;
                var Report = new SalesReport(res);
                _logger.LogInformation("Got sales report for invoice {invNo}", invNo);
                var ret = Report.GeneratePdf();
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales report for invoice {invNo}, request from {Name}", invNo, User.Identity.Name);
                return null;
            }
        }
    }
}