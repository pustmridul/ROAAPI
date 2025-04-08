using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class ServiceSale : BaseEntity
    {
        public int? MemberId { get; set; }
        public string InvoiceNo { get; set; }= string.Empty;
        public string? MembershipNo { get; set; }
        public string? OrderFrom { get; set; }
        public DateTime InvoiceDate { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServiceChargeAmount { get; set; }

        public string? SaleStatus { get; set; }
        public string? Note { get; set; }
        public RegisterMember? RegisterMember { get; set; }
        public ICollection<ServiceSaleDetail> ServiceSaleDetails { get; set; }

    }
}
