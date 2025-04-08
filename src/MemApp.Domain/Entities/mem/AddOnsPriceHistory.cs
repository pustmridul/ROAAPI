using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class AddOnsPriceHistory :BaseEntity
    {
        public bool IsActive { get; set; }
        public int AddOnsItemId { get; set; }
        public decimal Price { get; set; }
        public DateTime PriceDate { get; set; }
        public bool ActiveStatus { get; set; }

    }
}
