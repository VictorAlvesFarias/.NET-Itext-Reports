using Application.Dtos.ItextFile;
using iText.Html2pdf;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Itext
{
    public class ItextService : IItextService
    {
        private readonly IRazorService _razorService;
        public ItextService(IRazorService razorService)
        {
            _razorService = razorService;
        }

        public ItextFileResponse GenerateReportHtml(string html)
        {
            byte[] pdfBytes;

            using (var memoryStream = new MemoryStream())
            {
                ConverterProperties converterProperties = new ConverterProperties();
                HtmlConverter.ConvertToPdf(html, memoryStream, converterProperties);
                pdfBytes = memoryStream.ToArray();
            }

            var base64Pdf = Convert.ToBase64String(pdfBytes);
            var fileName = "report.pdf";
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
