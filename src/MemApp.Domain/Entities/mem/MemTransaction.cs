using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class MemTransaction : BaseEntity
    {
        public DateTime TransActionDate { get; set; }
        public string TransactionNo { get; set; }
        public string TransactionType { get; set; }
        public decimal TransactionAmount { get; set; }
        public int MemberId { get; set; }
        public string MemberShipNo { get; set; }
        public string TransactionMode { get; set; }
        public string TransactionStatus { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public int? ConfirmBy { get; set; }
        public string? ConfirmByName { get; set; }
        public DateTime? Confirmed { get; set; }

    }
}
