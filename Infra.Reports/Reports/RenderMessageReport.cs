using Application.Dtos.Document;
using Application.Extensions;
using Infra.Reports.Razor.Reports.RenderMessage;
using iText.Kernel.Pdf;
using iText.Layout;
using Reports.Reports.Shared;

namespace Reports.Reports
{
    public class RenderMessageReport : BaseReport
    {
        public RenderMessageReport(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public RenderMessageReport Generate()
        {
            Build(new Margins(40, 40, 40, 40), 150, 150);

            return this;
        }

        protected override void Body(Document doc, PdfWriter writer, PdfDocument pdfDoc)
        {
            var pdfStringBody = Render<RenderMessage>(new Dictionary<string, object?>
                {
                    { "Message", "Testando, Body do relatorio" },
                    { "Title","Lorem Ipsum" }
                }
            ).Result;

            doc.AddHtml(pdfStringBody);
        }

        protected override void Footer(Document doc, PdfWriter writer, PdfDocument pdfDoc)
        {
            doc.SetMargins(40, 40, 40, 40);

            var pdfStringFooter = Render<RenderMessageFooter>(new Dictionary<string, object?>
                {
                    { "Message", "Testando,hooter do relatório" },
                    { "Title","Footer" }
                }
            ).Result;
            
            doc.AddHtml(pdfStringFooter);
        }

        protected override void Header(Document doc, PdfWriter writer, PdfDocument pdfDoc)
        {
            doc.SetMargins(40, 40, 40, 40);

            var pdfStringHeader = Render<RenderMessageHeader>(new Dictionary<string, object?>
                {
                    { "Message", "Testando, header do relatorio" },
                    { "Title","Header" }
                }
            ).Result;
            
            doc.AddHtml(pdfStringHeader);
        }
    }
}
