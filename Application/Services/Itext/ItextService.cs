using Application.Dtos.ItextFile;
using Application.Helpers;
using iText.Forms.Fields.Merging;
using iText.Html2pdf;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Application.Services.Itext
{
    public class ItextService : IItextService
    {
        private readonly IRazorService _razorService;

        public ItextFileResponse GenerateReportHtml(DocumentConfig documentConfig, string fileName = "")
        {
            byte[] pdfBytes;

            using (var memoryStream = new MemoryStream())
            {
                var converterProperties = new ConverterProperties();
                var writer = new PdfWriter(memoryStream);
                var pdfDocument = new PdfDocument(writer);
                var document = new Document(pdfDocument, documentConfig.PageSize, false);
                var docTable = new Table(UnitValue.CreatePercentArray(1))
                       .UseAllAvailableWidth().SetBorder(Border.NO_BORDER)
                       .SetMinHeight(documentConfig.PageSize.GetHeight())
                       .SetMargin(0)
                       .SetPadding(0);
                        
                var list = HtmlConverter.ConvertToElements(documentConfig.Html, converterProperties);

                document.SetMargins(documentConfig.Margins.Left, documentConfig.Margins.Right, documentConfig.Margins.Top, documentConfig.Margins.Bottom);

                foreach (var item in list)
                {
                    var cell = new Cell()
                        .SetMargin(0)
                        .SetPadding(0)
                        .SetBorder(Border.NO_BORDER)
                        ;
                    cell.Add((IBlockElement)item);
                    docTable.AddCell(cell);   
                }
                document.Add(docTable);
                document.Close();

                pdfBytes = memoryStream.ToArray();
            }

            var base64Pdf = Convert.ToBase64String(pdfBytes);
            var response = new ItextFileResponse
            {
                Base64 = base64Pdf,
                FileName = fileName,
                File = pdfBytes,
                Type = "application/pdf"
            };

            return response;
        }
        public ItextFileResponse Generate(string fileName, HeaderDocument headerDocumentConfig, FooterDocument footerDocumentConfig, BodyDocument bodyDocumentConfig)
        {
            byte[] pdfBytes;

            var headerPdf = GenerateReportHtml(headerDocumentConfig);
            var footerPdf = GenerateReportHtml(footerDocumentConfig);
            var bodyPdf = GenerateReportHtml(bodyDocumentConfig);

            using (var documentMemoryStream = new MemoryStream())
            using (var bodyMemoryStream = new MemoryStream(bodyPdf.File))
            using (var footerMemoryStream = new MemoryStream(footerPdf.File))
            using (var headerMemoryStream = new MemoryStream(headerPdf.File))
            {
                var writer = new PdfWriter(documentMemoryStream);
                var pdfDocument = new PdfDocument(writer);
                var bodyReader = new PdfReader(bodyMemoryStream);
                var bodyPdfDocument = new PdfDocument(bodyReader);
                var footerReader = new PdfReader(footerMemoryStream);
                var footerPdfDocument = new PdfDocument(footerReader);
                var headerReader = new PdfReader(headerMemoryStream);
                var headerPdfDocument = new PdfDocument(headerReader);
                var numberOfPages = bodyPdfDocument.GetNumberOfPages();
                var footerPage = footerPdfDocument.GetFirstPage();
                var headerPage = headerPdfDocument.GetFirstPage();

                pdfDocument.SetDefaultPageSize(PageSize.A4);

                for (int i = 1; i <= numberOfPages; i++)
                {
                    var bodyPage = bodyPdfDocument.GetPage(i);
                    var newPage = pdfDocument.AddNewPage();
                    var bodyPdfCanvas = new PdfCanvas(newPage);
                    var bodyCanvas = new Canvas(bodyPdfCanvas, bodyPage.GetPageSize());
                    var footerPdfCanvas = new PdfCanvas(newPage);
                    var footerCanvas = new Canvas(footerPdfCanvas, footerPage.GetPageSize());
                    var headerPdfCanvas = new PdfCanvas(newPage);
                    var headerCanvas = new Canvas(headerPdfCanvas, headerPage.GetPageSize());
                    var copyBodyPage = bodyPage.CopyAsFormXObject(pdfDocument);
                    var copyFooterPage = footerPage.CopyAsFormXObject(pdfDocument);
                    var copyHeaderPage = headerPage.CopyAsFormXObject(pdfDocument);

                    headerPdfCanvas.AddXObjectAt(copyHeaderPage,0, newPage.GetPageSize().GetHeight() - headerPage.GetPageSize().GetHeight());
                    footerPdfCanvas.AddXObjectAt(copyFooterPage,0,0);
                    bodyPdfCanvas.AddXObjectAt( copyBodyPage,0,newPage.GetPageSize().GetHeight() - bodyPage.GetPageSize().GetHeight() - headerPage.GetPageSize().GetHeight());
                }

                pdfDocument.Close();
                pdfBytes = documentMemoryStream.ToArray();
            }

            var base64Pdf = Convert.ToBase64String(pdfBytes);
            var response = new ItextFileResponse
            {
                Base64 = base64Pdf,
                FileName = fileName,
                File = pdfBytes,
                Type = "application/pdf"
            };

            return response;
        }

    }
}
