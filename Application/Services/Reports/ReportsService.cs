using Application.Dtos.ItextFile;
using Application.Helpers;
using Application.Services.Itext;
using Infra.Reports.Razor.Helpers;
using Infra.Reports.Razor.Reports.RenderMessage;
using iText.Kernel.Geom;

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
            var pdfStringBody = _razorService.Render<RenderMessage>( new Dictionary<string, object?>
                {
                    { "Message", "Testando, header do relatorio" },
                    { "Title","Lorem Ipsum" }
                }
            ).Result;

            var pdfStringHeader = _razorService.Render<RenderMessageHeader>(new Dictionary<string, object?>
                {
                    { "Message", "Testando, header do relatorio" },
                    { "Title","Header" }
                }
            ).Result;

            var pdfStringFooter = _razorService.Render<RenderMessageFooter>(new Dictionary<string, object?>
                {
                    { "Message", "Testando,hooter do relatório" },
                    { "Title","Footer" }
                }
            ).Result;

            var headerHeight =35f;
            var footerHeight = 35f;
            var bodyHeight = PageSize.A4.GetHeight() - headerHeight - headerHeight +5f ; 


            var body = new BodyDocument()
            {
                Margins = new Margins(40, 40, 40, 40),
                Html = pdfStringBody,
                PageSize = new PageSize(PageSize.A4.GetWidth(), bodyHeight)
            };
            var header = new HeaderDocument()
            {
                Margins = new Margins(0, 0, 0, 0),
                Html = pdfStringHeader,
                PageSize = new PageSize(PageSize.A4.GetWidth(), headerHeight)
            };
            var footer = new FooterDocument()
            {
                Margins = new Margins(0, 0, 0 ,0),
                Html = pdfStringFooter,
                PageSize = new PageSize(PageSize.A4.GetWidth(), footerHeight)
            };

            var result = _itextService.Generate("relatorioTeste.pdf",header,footer,body);

            return result;
        }
    }
}
