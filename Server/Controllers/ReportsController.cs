using AccReporting.Server.Data;
using AccReporting.Server.Reports;
using AccReporting.Server.Services;
using AccReporting.Shared.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuestPDF.Fluent;

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

        [AllowAnonymous]
        [HttpGet("InvSummaryListPaged")]
        public async Task<IEnumerable<InvSummGridModel>> InvSummaryListPaged(int page, CancellationToken ct, int pageSize = 25)
        {
            await _dataService.SetDbName("Test", ct);
            return await _dataService.GetInvSummGridAsync("2.1.4.154", page, pageSize, ct);
        }

        [AllowAnonymous]
        [HttpGet("InvSummaryList")]
        public async Task<IEnumerable<InvSummGridModel>> InvSummaryList(CancellationToken ct)
        {
            await _dataService.SetDbName("Test", ct);
            return await _dataService.GetInvSummGridAsync("2.1.4.154", 0, 0, ct);
        }

        [AllowAnonymous]
        [HttpGet("SalesReport")]
        public async Task<IActionResult> SalesReport(int invNo, string type, CancellationToken ct)
        {
            try
            {
                string InpType = types[type];
                invNo.Throw()
                            .IfNegative()
                            .IfDefault();
                type.Throw()
                    .IfNullOrWhiteSpace(x => x);

                _logger.LogInformation("Getting sales report for invoice {invNo}", invNo);
                await _dataService.SetDbName("Test", ct);
                var res = await _dataService.GetSalesInvoiceData(invNo, InpType, "2.1.4.154", ct);
                if (res?.InvNo != invNo)
                {
                    return NotFound($"Error: Invoice number {invNo} not found");
                }
                res.Type = type;
                res.cell = "0345-5551092";
                res.Address = "Allah wala town, Korangi crossing";
                var Report = new SalesReport(res);
                _logger.LogInformation("Got sales report for invoice {invNo}", invNo);
                return File(Report.GeneratePdf(), "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting sales report for invoice {invNo}, request from {User.Identity.Name}");
                return Redirect("/Error");
            }
        }
    }
}