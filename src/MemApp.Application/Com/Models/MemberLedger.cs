using MemApp.Application.Extensions;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Com.Models
{
    public class MemberLedgerReq 
    {
        public decimal CustomerLedgerID { get; set; }
        public string PrvCusID { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Dates { get; set; }
        public string UpdateBy { get; set; } = string.Empty;
        public DateTime UpdateDate { get; set; }
        public string TOPUPID { get; set; } = string.Empty;
        public DateTime DatesTime { get; set; }
        public string PayType { get; set; } = string.Empty;
        public string BankCreditCardName { get; set; } = string.Empty;
        public string ChequeCardNo { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string RefundId { get; set; } = string.Empty;        
        public decimal ServiceChargeAmount { get; set; }
    }

    public class MemberLedgerListVm : Result
    {
        public List <MemberLedgerReq> DataList { get; set; } = new List <MemberLedgerReq> () ;     
        public int DataCount { get; set; }

    }
}
