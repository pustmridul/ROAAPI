using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Models
{
    public class ExportVm
    {
        public ExportVm(string fileName, string contentType, byte[] content)
        {
            FileName = fileName;
            ContentType = contentType;
            Content = content;
        }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }
}
