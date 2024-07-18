using Application.Dtos.ItextFile;
using Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Itext
{
    public interface IItextService
    {
        ItextFileResponse GenerateReportHtml(DocumentConfig html, string fileName);
        ItextFileResponse Generate(string fileName, HeaderDocument headerDocument, FooterDocument footerDocument, BodyDocument bodyDocument);
    }
}
