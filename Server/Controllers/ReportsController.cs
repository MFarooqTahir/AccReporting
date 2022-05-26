using AccReporting.Server.Services;
using AccReporting.Shared.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccReporting.Server.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<SalesReportDto> SalesReport(int invNo, string type)
        {
            try
            {
                _logger.LogInformation("Getting sales report for invoice {invNo}", invNo);
                await _dataService.SetDbName("Test");
                _dataService.AccountNumber = "";
                var res = await _dataService.GetSalesInvoiceData(invNo, type);
                _logger.LogInformation("Got sales report for invoice {invNo}", invNo);
                return res;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}