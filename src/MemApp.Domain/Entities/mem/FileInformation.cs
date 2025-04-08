using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class FileInformation:BaseEntity
    {
        public string? FileContent { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? FileSize { get; set; }
        public string? FileType { get; set; }
        public bool IsActive { get; set; }
        public Byte[]? File { get; set; }
        public int OperationTypeId { get; set; }
        public int OperationId { get; set; }

    }
}
