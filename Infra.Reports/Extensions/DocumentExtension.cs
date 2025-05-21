
using iText.Html2pdf;
using iText.Layout;
using iText.Layout.Element;

namespace Application.Extensions
{
    public static class DocumentExtension
    {
        public static Document AddHtml(this Document doc, string html)
        {
            var converterProperties = new ConverterProperties();
            var list = HtmlConverter.ConvertToElements(html, converterProperties);
            
            foreach (var item in list)
            {
                doc.Add((IBlockElement)item);
            }
            
            return doc;
        }
    }
}
