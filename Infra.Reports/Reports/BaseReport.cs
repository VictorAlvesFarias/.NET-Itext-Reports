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
using Application.Extensions;
using iText.Layout.Properties;

namespace Reports.Reports.Shared
{
    public class BaseReport
    { 
        protected IServiceProvider _serviceProvider { get; set; }
        public string Base64 { get; set; }
        public string FileName { get; set; }
        public HttpResponseMessage File { get; set; }
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

                File = new HttpResponseMessage()
                {
                    Content = new ByteArrayContent(pdfBytes)
                };
                File.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = name + ".pdf"
                };
                File.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

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
        public static Text CreateT(string s, Func<Text, Text> styles = null, bool trim = true)
        {
            if (trim)
            {
                var newText = s.Trim();
                return styles is not null ? styles(new Text(newText + " ")) : new Text(newText + " ");
            }

            return styles is not null ? styles(new Text(s + " ")) : new Text(s + " ");
        }
        public static Paragraph CreateP(List<Text> texts, Func<Paragraph, Paragraph> styles = null)
        {
            var p = new Paragraph();

            foreach (var text in texts)
            {
                p.Add(text);
            }

            return styles is not null ? styles(p) : p;
        }
        protected List<Paragraph> P(List<Text> texts, Func<Paragraph, Paragraph> styles = null)
        {
            return new List<Paragraph>() { CreateP(texts, styles) };
        }
        protected List<Text> Strong(string s, Func<Text, Text> styles = null, bool trim = true)
        {
            var text = CreateT(s, styles, trim);
            return new List<Text>() { text.SimulateBold() };
        }
        protected List<Text> T(string s, Func<Text, Text> styles = null, bool trim = true)
        {
            return new List<Text>() { CreateT(s, styles, trim) };
        }
        protected List<Text> Br(Func<Text, Text> styles = null)
        {
            return new List<Text>() { CreateT("\r\n", styles, true) };
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
        public static Cell CreateCell(IBlockElement content, Func<Cell, Cell> styles = null, int colSpan = 1, int rowSpan = 1)
        {
            var cell = new Cell(rowSpan, colSpan).Add(content);
            return styles is not null ? styles(cell) : cell;
        }
        public static Table CreateTable(UnitValue[] columnWidths, Func<Table, Table> styles = null)
        {
            var table = new Table(columnWidths);
            return styles is not null ? styles(table) : table;
        }
        protected List<Cell> C(IBlockElement content, Func<Cell, Cell> styles = null, int colSpan = 1, int rowSpan = 1)
        {
            return new List<Cell>() { CreateCell(content, styles, colSpan, rowSpan) };
        }
        protected List<Table> T(UnitValue[] columnWidths, Func<Table, Table> styles = null)
        {
            return new List<Table>() { CreateTable(columnWidths, styles) };
        }
    }
}