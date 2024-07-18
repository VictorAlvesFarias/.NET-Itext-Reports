using Application.Services.Itext;
using Application.Services.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Relatorios_Cshtml.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _reportsService;
        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }
        [HttpGet("test")]
        public IActionResult Test()
        {
            var response = _reportsService.TestReport();

            return File(response.File, response.Type, response.FileName);
        }
    }
}
