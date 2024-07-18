using iText.Kernel.Events;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf;
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
    public class HeaderEventHandler : IEventHandler
    {
        private string header;
        public HeaderEventHandler(string header)
        {
            this.header = header;
        }

        public virtual void HandleEvent(Event @event)
        {
            var docEvent = (PdfDocumentEvent)@event;
            var pdf = docEvent.GetDocument();
            var page = docEvent.GetPage();
            var pageSize = page.GetPageSize();
            var canvas = new Canvas(new PdfCanvas(page), pageSize);

            canvas.SetFontSize(18);
            canvas.ShowTextAligned(header,
                pageSize.GetWidth() / 2,
                pageSize.GetTop() - 30, TextAlignment.CENTER);
            canvas.Close();
        }
    }
}
