using Application.Dtos.Document;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using iText.Kernel.Pdf.Event;
using iText.Kernel.Colors;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Reports.Reports.Shared
{
    public class BaseReport
    { 
        protected IServiceProvider _serviceProvider { get; set; }
        public string Base64 { get; set; }
        public string FileName { get; set; }
        public HttpResponseMessage FilePFD { get; set; }
        public Margins Margins { get; set; }

        protected string Build(Margins margins, float footerHeight, float headerHeight, Margins footerMargins = null, PageSize pageSize = null)
        {
            pageSize = pageSize ?? PageSize.A4;

            Margins = margins;
            
            var name = Guid.NewGuid().ToString();

            using (var streamFinal = new MemoryStream())
            {
                var writer = new PdfWriter(streamFinal);
                var pdfDoc = new PdfDocument(writer);
                var doc = new Document(pdfDoc, PageSize.DEFAULT, false);

                PdfFormXObject headerForm;
                PdfFormXObject footerForm;


                using (var headerStream = new MemoryStream())
                {
                    var headerWriter = new PdfWriter(headerStream);
                    var headerPdfDoc = new PdfDocument(headerWriter);
                    var headerPageSize = new PageSize(pageSize.GetWidth(), headerHeight);
                    var headerDoc = new Document(headerPdfDoc, headerPageSize, false);

                    Header(headerDoc, headerWriter, headerPdfDoc);

                    headerDoc.Close();
                    headerPdfDoc = new PdfDocument(new PdfReader(new MemoryStream(headerStream.ToArray())));
                    
                    var headerPage = headerPdfDoc.GetFirstPage();
                    
                    headerHeight = headerPage.GetPageSize().GetHeight();
                    headerForm = headerPage.CopyAsFormXObject(pdfDoc);
                }

                using (var footerStream = new MemoryStream())
                {
                    var footerWriter = new PdfWriter(footerStream);
                    var footerPdfDoc = new PdfDocument(footerWriter);
                    var footerPageSize = new PageSize(pageSize.GetWidth(), footerHeight);
                    var footerDoc = new Document(footerPdfDoc, footerPageSize, false);

                    footerDoc.SetBackgroundColor(ColorConstants.BLUE);

                    Footer(footerDoc, footerWriter, footerPdfDoc);
                    
                    footerDoc.Close();
                    footerPdfDoc = new PdfDocument(new PdfReader(new MemoryStream(footerStream.ToArray())));
                    
                    var footerPage = footerPdfDoc.GetFirstPage();

                    footerHeight = footerPage.GetPageSize().GetHeight();
                    footerForm = footerPage.CopyAsFormXObject(pdfDoc);
                }

                pdfDoc.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandler(headerForm, headerHeight));
                pdfDoc.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandler(footerForm, footerHeight));

                doc.SetMargins(margins.Top + headerHeight, margins.Right, margins.Bottom + footerHeight, margins.Left);

                Body(doc, writer, pdfDoc);

                var pagesNumber = GetPages(doc, pdfDoc, margins);
                var pageHeight = PageSize.DEFAULT.GetHeight();

                doc.Close();

                var pdfBytes = streamFinal.ToArray();
                var base64String = Convert.ToBase64String(pdfBytes);

                FilePFD = new HttpResponseMessage()
                {
                    Content = new ByteArrayContent(pdfBytes)
                };
                FilePFD.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = name + ".pdf"
                };
                FilePFD.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                Base64 = base64String;
                FileName = name + ".pdf";

                return base64String;
            }
        }
        protected virtual void Body(Document doc, PdfWriter writer, PdfDocument pdfDoc)
        {

        }
        protected virtual void Header(Document doc, PdfWriter writer, PdfDocument pdfDoc)
        {

        }
        protected virtual void Footer(Document doc, PdfWriter writer, PdfDocument pdfDoc)
        {

        }
        protected virtual void Footer(Document doc)
        {

        }
        protected Text Strong(string s)
        {
            var newText = s.Trim();

            return new Text(newText + " ").SimulateBold();
        }
        protected Text T(string s, bool trim = true)
        { 
            if (trim)
            {
                var newText = s.Trim();

                return new Text(newText + " ");
            }

            return new Text(s + " ");
        }
        protected Text Br()
        {
            return new Text("\r\n");
        }
        protected Paragraph P(List<Text> list)
        {
            var paragraph = new Paragraph();

            list.ForEach(x =>
            {
                paragraph.Add(x);
            });

            return paragraph;
        }
        protected Paragraph P(Text list)
        {
            var paragraph = new Paragraph();

            paragraph.Add(list);

            return paragraph;
        }
        protected Cell Cell(Paragraph p)
        {
            var cell = new Cell();
            cell.Add(p);
            return cell;
        }
        protected Cell Cell(Table p)
        {
            var cell = new Cell();
            cell.Add(p);
            return cell;
        }
        protected int GetPages(Document doc, PdfDocument pdfDoc, Margins margins)
        {
            return doc.GetPdfDocument().GetNumberOfPages();
        }
        protected async Task<string> Render<T>(Dictionary<string, object?> dictionary) where T : IComponent
        {
            await using var htmlRenderer = new HtmlRenderer(_serviceProvider, _serviceProvider.GetRequiredService<ILoggerFactory>());

            var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var parameters = ParameterView.FromDictionary(dictionary);
                var output = await htmlRenderer.RenderComponentAsync<T>(parameters);

                return output.ToHtmlString();
            });

            return html;
        }
    }
}

