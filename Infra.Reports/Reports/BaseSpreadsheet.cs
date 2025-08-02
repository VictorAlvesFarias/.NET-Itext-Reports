using ClosedXML.Excel;
using OfficeOpenXml;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Reports.Spreadsheets.Shared
{
    public abstract class BaseSpreadsheet
    {
        public string Base64 { get; set; }
        public string FileName { get; set; }
        public HttpResponseMessage File { get; set; }

        public string Generate()
        {
            using (var workbook = new XLWorkbook())
            {
                Spreadsheed(workbook);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    byte[] excelBytes = stream.ToArray();
                    string base64String = Convert.ToBase64String(excelBytes);
                    var name = Guid.NewGuid().ToString();

                    File = new HttpResponseMessage()
                    {
                        Content = new ByteArrayContent(excelBytes)
                    };

                    File.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = name + ".xlsx"
                    };

                    File.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                    Base64 = base64String;
                    FileName = name + ".xlsx";

                    return base64String;
                }
            }
        }
        public abstract void Spreadsheed(XLWorkbook workbook);
    }
    public class ExcelTable
    {
        private int Collunm { get; set; }
        private int Collunms { get; set; }
        private int Row { get; set; }
        private int CurrentCollunm { get; set; }
        private int CurrentRow { get; set; }
        private IXLWorksheet Worksheet { get; set; }

        public ExcelTable(int collunms, int startCollunm, int startRow, IXLWorksheet worksheet)
        {
            Worksheet = worksheet;
            Collunm = startCollunm;
            Row = startRow;
            CurrentCollunm = 0;
            CurrentRow = 0;
            Collunms = collunms;
        }

        public ExcelTable Cell(string text, Action<IXLStyle> style = null)
        {
            var cell = Worksheet.Cell(CurrentRow + Row, CurrentCollunm + Collunm);
            
            cell.Value = text;

            if (style is not null)
            {
                style(cell.Style);
            }

            CurrentCollunm++;

            if (CurrentCollunm >= Collunms)
            {
                CurrentCollunm = 0;
                CurrentRow++;
            }

            return this;
        }
    }
}
