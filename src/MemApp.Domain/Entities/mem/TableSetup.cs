using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class TableSetup : BaseEntity
    {
        public string  Title { get; set; }
        public string? DisplayName { get; set; }
        public bool IsActive { get; set; }
        public ICollection<AreaLayoutDetail> AreaLayoutDetails { get; set; }
        public ICollection<AreaTableMatrix> AreaTableMatrices { get; set; }

    }
}
