using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.Sale
{
    public class MiscSale : BaseEntity
    {
        public string InvoiceNo { get; set; }= string.Empty;
        public DateTime InvoiceDate { get; set; }
        public int? MemberId { get; set; }
        public RegisterMember? RegisterMember { get; set; }
        public string? Note { get; set; }
        public ICollection<MiscSaleDetail> MiscSaleDetails { get; set; }
    }
}
