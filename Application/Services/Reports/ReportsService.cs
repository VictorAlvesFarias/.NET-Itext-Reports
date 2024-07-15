using Application.Dtos.ItextFile;
using Application.Services.Itext;
using Infra.Reports.Razor.Reports.RenderMessage;

namespace Application.Services.Reports
{
    public class ReportsService: IReportsService
    {
        private readonly IItextService _itextService;
        private readonly IRazorService _razorService;

        public ReportsService(IItextService itextService, IRazorService razorService) 
        {
            _itextService = itextService;
            _razorService = razorService;
        }
        public ItextFileResponse TestReport()
        {
            var pdfString = _razorService.Render<RenderMessage>( new Dictionary<string, object?>
                {
                    { "Message", "Hello from the Render Message component!" }
                }
            ).Result;

            var result = _itextService.GenerateReportHtml(pdfString);

            return result;
        }
    }
}
