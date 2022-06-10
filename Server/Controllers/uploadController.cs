using AccReporting.Server.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Text;

namespace AccReporting.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class uploadController : ControllerBase
    {
        private readonly ILogger<uploadController> _logger;
        private readonly DataService _dataService;

        public uploadController(ILogger<uploadController> logger, DataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        [HttpPost("fileupload")]
        public async Task<ActionResult> fileupload(CancellationToken ct)
        {
            try
            {
                string text = string.Empty;
                var file = Request.Form.Files[0];
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
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Data Error:" + ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}