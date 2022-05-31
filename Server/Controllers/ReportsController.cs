using AccReporting.Server.Reports;
using AccReporting.Server.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QuestPDF.Fluent;

using System.Diagnostics;

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
                invNo.Throw()
                    .IfNegative();
                type.Throw().IfNullOrWhiteSpace(x => x);
                _logger.LogInformation("Getting sales report for invoice {invNo}", invNo);
                await _dataService.SetDbName("Test");
                _dataService.AccountNumber = "";
                var res = await _dataService.GetSalesInvoiceData(invNo, type, ct);
                var Report = new SalesReport(res);
                _logger.LogInformation("Got sales report for invoice {invNo}", invNo);
                return File(Report.GeneratePdf(), "application/pdf");
            }
            catch (Exception ex)
            {
                string ID = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
                _logger.LogError(ex, $"Error getting sales report for invoice {invNo}, request from {ID}");
                return Redirect("/Error");
            }
        }
    }
}