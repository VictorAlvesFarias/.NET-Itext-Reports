
using iText.Html2pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Reports.Reports.Shared;

namespace Application.Extensions
{
    public static class ItextExtensions
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
        public static List<Text> T(this List<Text> lt, string s, Func<Text, Text> styles = null, bool trim = true)
        {
            lt.Add(BaseReport.CreateT(s, styles, trim));
            return lt;
        }
        public static List<Text> Strong(this List<Text> lt, string s, Func<Text, Text> styles = null, bool trim = true)
        {
            lt.Add(BaseReport.CreateT(s, styles, trim).SimulateBold());
            return lt;
        }
        public static List<Text> Br(this List<Text> lt, Func<Text, Text> styles = null)
        {
            lt.Add(BaseReport.CreateT("\r\n", styles, true));
            return lt;
        }
        public static List<Paragraph> P(this List<Paragraph> lp, List<Text> texts, Func<Paragraph, Paragraph> styles = null)
        {
            lp.Add(BaseReport.CreateP(texts, styles));
            return lp;
        }
        public static List<Cell> C(this List<Cell> list, IBlockElement content, Func<Cell, Cell> styles = null, int colSpan = 1, int rowSpan = 1)
        {
            list.Add(BaseReport.CreateCell(content, styles, colSpan, rowSpan));
            return list;
        }
        public static List<Table> T(this List<Table> list, UnitValue[] columnWidths, Func<Table, Table> styles = null)
        {
            list.Add(BaseReport.CreateTable(columnWidths, styles));
            return list;
        }
    }
}
