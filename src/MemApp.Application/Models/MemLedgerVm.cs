using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Models
{
    public class MemLedgerVm
    {
        public decimal CustomerLedgerID { get; set; }
        public string PrvCusID { get; set; }
        public decimal? Amount { get; set; }
        public string? Description { get; set; }
        public DateTime? Dates { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? TOPUPID { get; set; }
        public DateTime? DatesTime { get; set; }
        public string? PayType { get; set; }
        public string? BankCreditCardName { get; set; }
        public string? ChequeCardNo { get; set; }
        public string? Notes { get; set; }
        public string? RefundId { get; set; }
        public decimal? ServiceChargeAmount { get; set; }
        public string? TransactionFrom { get; set; }
        public string? TransactionType { get; set; }
        public string? ReferenceId { get; set; }
    }
}
