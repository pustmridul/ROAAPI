using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.ser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class AddOnsItem : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime PriceDate { get; set; }
     
    }

}
