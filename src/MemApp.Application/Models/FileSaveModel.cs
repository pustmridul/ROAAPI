using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Models
{
    public class FileSaveModel
    {
        public bool IsUpdate { get; set; }
        public string SubDirectory { get; set; }
        public string WebRootPath { get; set; }
        public IFormFile file { get; set; }
        public int OperationTypeId { get; set; }
        public int OperationId { get; set; }
    }
}
