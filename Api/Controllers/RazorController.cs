using Infra.Reports.Razor.Reports.RenderMessage;
using Microsoft.AspNetCore.Mvc;

namespace Relatorios_Cshtml.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RazorController : ControllerBase
    {
        private readonly IRazorService _razorService;
        public RazorController(IRazorService razorService)
        {
            _razorService = razorService;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(_razorService.Render<RenderMessage>(new Dictionary<string, object?>
                {
                    { "Message", "Hello from the Render Message component!" }
                }
            ).Result);
        }
    }
}
