using Infra.Reports.Razor.Helpers;
using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class DocumentConfig
    {
        public string Html { get; set; }
        public Margins Margins { get; set; }
        public PageSize PageSize { get; set; }
    }
}
