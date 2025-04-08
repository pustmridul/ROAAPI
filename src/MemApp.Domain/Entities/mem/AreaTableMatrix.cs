using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class AreaTableMatrix : BaseEntity
    {
        public int AreaLayoutId { get; set; }
        public int TableId { get; set; }
        public int ChairNo { get; set; } 
        public string? ChairKeyNo { get; set; }
        public bool IsActive { get; set; }

        public AreaLayout AreaLayout { get; set; }
        public TableSetup TableSetup { get; set; }
    }
}
