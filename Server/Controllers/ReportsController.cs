using AccReporting.Server.Data;
using AccReporting.Server.Reports;
using AccReporting.Server.Services;
using AccReporting.Shared;
using AccReporting.Shared.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using QuestPDF.Fluent;

using System.Security.Claims;

using Throw;

namespace AccReporting.Server.Controllers
{
    [Authorize]
    [Route(template: "[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _authDb;
        private readonly ILogger<ReportsController> _logger;
        private readonly DataService _dataService;

        private static readonly IDictionary<string, string> Types = new Dictionary<string, string>()
                {
                    {"Sale","S" },{"Estimate","E" },{"Purchase","P" },{"Return","R"},{"Purchase Return","Q"},{"Cash","C"}
                };

        public ReportsController(DataService dataService, ILogger<ReportsController> logger, ApplicationDbContext authDb)
        {
            _dataService = dataService;
            _logger = logger;
            _authDb = authDb;
        }

        [HttpGet(template: "InvSummaryList")]
        public async Task<IEnumerable<InvSummGridModel>> InvSummaryListPaged(string type, CancellationToken ct, int page = 0, int pageSize = 0)
        {
            try
            {
                var id = User.Claims.First(predicate: a => a.Type == ClaimTypes.NameIdentifier).Value;
                var data = _authDb.CompanyAccounts.Where(predicate: x => x.UserId == id && x.IsSelected)
                    .Select(selector: y => new { y.AcNumber, y.Company.DbName }).First();
                await _dataService.SetDbName(dbName: data.DbName, ct: ct);
                var ret = await _dataService.GetInvSummGridAsync(acCode: data.AcNumber, type: type, pageNumber: page, pageSize: pageSize, ct: ct);
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(message: "There was an error {Message}", ex.Message);
                return null;
            }
        }

        [HttpGet(template: "[action]")]
        public async Task<FileResponse> SalesReport(int invNo, string type, string pcode, CancellationToken ct)
        {
            try
            {
                _ = Request;
                var inpType = Types[key: type];
                invNo.Throw()
                            .IfNegative()
                            .IfDefault();
                type.Throw()
                    .IfNullOrWhiteSpace(func: x => x);
                var id = User.Claims.First(predicate: a => a.Type == ClaimTypes.NameIdentifier).Value;
                var data = await _authDb.CompanyAccounts
                    .Where(predicate: x => x.UserId == id && x.IsSelected)
                    .Select(selector: y => new { y.AcNumber, y.Company.DbName, y.Company.Name, y.Company.Phone, y.Company.Address })
                    .FirstAsync(cancellationToken: ct);
                _logger.LogInformation(message: "Getting sales report for invoice {invNo}", invNo);
                await _dataService.SetDbName(dbName: data.DbName, ct: ct);
                var res = await _dataService.GetSalesInvoiceData(invNo: invNo, type: inpType, acNumber: data.AcNumber, pcode: pcode, ct: ct);
                if (res?.InvNo != invNo)
                {
                    _logger.LogInformation(message: "Invoice not found {invNo}", invNo);
                    return null;
                }
                res.Type = type;
                res.CompanyName = data.Name;
                res.Cell = data.Phone;
                res.Address = data.Address;
                var report = new SalesReportQuest(reportData: res);
                _logger.LogInformation(message: "Got sales report for invoice {invNo}", invNo);
                return new FileResponse() { File = report.GeneratePdf(), Name = "SalesReport.pdf" };
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, message: "Error getting sales report for invoice {invNo}, request from {Name}", invNo, User.Identity.Name);
                return null;
            }
        }
    }
}