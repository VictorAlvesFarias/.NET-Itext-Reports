using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.ItextFile
{
    public class ItextFileResponse
    {
        public string Base64 { get; set; }
        public string FileName { get; set; }
        public byte[] File { get; set; }
        public string Type { get; set; }
    }
}
