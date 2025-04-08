using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.Sale
{
    public class MiscSaleDetail : BaseEntity
    {
        public int MiscSaleId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public MiscItem MiscItem { get; set; }
        public MiscSale MiscSale { get; set; }
    }
}
