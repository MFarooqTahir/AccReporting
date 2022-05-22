using AccReporting.Server.Services;

using Microsoft.AspNetCore.Mvc;

using System.Text;

namespace AccReporting.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class uploadController : ControllerBase
    {
        private readonly ILogger<uploadController> _logger;
        private readonly DataService _dataService;

        public uploadController(ILogger<uploadController> logger, DataService dataService)
        {
            this._logger = logger;
            _dataService = dataService;
        }

        [HttpPost("fileupload")]
        public async Task<ActionResult> fileupload()
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
                await _dataService.SetDbName("Test");
                var res = await _dataService.InsertAllDataBulk(text);
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