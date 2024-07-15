using Application.Dtos.ItextFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Itext
{
    public interface IItextService
    {
        ItextFileResponse GenerateReportHtml(string html);
    }
}
