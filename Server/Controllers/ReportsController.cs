using AccReporting.Server.Reports;
using AccReporting.Server.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuestPDF.Fluent;

using Throw;

namespace AccReporting.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly DataService _dataService;

        public ReportsController(DataService dataService, ILogger<ReportsController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("SalesReport")]
        public async Task<IActionResult> SalesReport(int invNo, string type, CancellationToken ct)
        {
            try
            {
                IDictionary<string, string> types = new Dictionary<string, string>()
                {
                    {"Sale","S" },{"Estimate","E" },{"Purchase","P" },{"Return","R"}
                };
                string InpType = types[type];
                invNo.Throw()
                            .IfNegative();
                type.Throw()
                    .IfNullOrWhiteSpace(x => x);

                _logger.LogInformation("Getting sales report for invoice {invNo}", invNo);
                await _dataService.SetDbName("Test", ct);
                _dataService.AccountNumber = "2.1.4.154";
                var res = await _dataService.GetSalesInvoiceData(invNo, InpType, ct);
                if (res?.InvNo != invNo)
                {
                    return NotFound($"Error: Invoice number {invNo} not found");
                }
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