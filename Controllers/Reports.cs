using Microsoft.AspNetCore.Mvc;

namespace Relatorios_Cshtml.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Reports : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }

    }
}
