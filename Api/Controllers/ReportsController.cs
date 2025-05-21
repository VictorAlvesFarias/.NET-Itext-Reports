using Microsoft.AspNetCore.Mvc;
using Reports.Reports;

namespace Relatorios_Cshtml.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly RenderMessageReport _report;
        public ReportsController(RenderMessageReport report)
        {
            _report = report;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }
        [HttpGet("test")]
        public IActionResult Test()
        {
            var base64 = _report.Generate().Base64;
            var fileBytes = Convert.FromBase64String(base64);
            var contentType = "application/pdf"; // ou o tipo correto do arquivo
            var fileName = "report.pdf";

            return File(fileBytes, contentType, fileName);
        }

    }
}
