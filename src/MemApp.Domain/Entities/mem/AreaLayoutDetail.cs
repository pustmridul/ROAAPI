using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class AreaLayoutDetail : BaseEntity
    {
        public string TableName { get; set; } = string.Empty;
        public int TableId { get; set; }
        public TableSetup TableSetup { get; set; }
        public int? NumberOfChair { get; set; }
        public int AreaLayoutId { get; set; }
        public bool IsActive { get; set; }
        public AreaLayout AreaLayout { get; set; }
    }
}
