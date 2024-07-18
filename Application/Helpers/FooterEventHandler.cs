using iText.Kernel.Events;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Xobject;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Geom;

namespace Infra.Reports.Razor.Helpers
{
    public class FooterEventHandler : IEventHandler
    {
        protected PdfFormXObject placeholder;
        protected float side = 20;
        protected float x = 300;
        protected float y = 25;
        protected float space = 4.5f;
        protected float descent = 3;
        public FooterEventHandler()
        {
            placeholder = new PdfFormXObject(new Rectangle(0, 0, side, side));
        }

        public virtual void HandleEvent(Event @event)
        {
            var docEvent = (PdfDocumentEvent)@event;
            var pdf = docEvent.GetDocument();
            var page = docEvent.GetPage();
            var pageNumber = pdf.GetPageNumber(page);
            var pageSize = page.GetPageSize();
            var pdfCanvas = new PdfCanvas(page);
            var canvas = new Canvas(pdfCanvas, pageSize);

            var p = new Paragraph()
                .Add("Page ")
                .Add(pageNumber.ToString())
                .Add(" of");

            canvas.Add(p);

            canvas.Close();
        }
    }
}
