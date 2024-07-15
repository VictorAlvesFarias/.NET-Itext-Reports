using Microsoft.AspNetCore.Mvc;

namespace Relatorios_Cshtml.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Razor : ControllerBase
    {
        private readonly IRazorService _razorService ;
        public Razor(IRazorService razorService) 
        { 
            _razorService = razorService;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(_razorService.Render().Result);
        }
    }
}
