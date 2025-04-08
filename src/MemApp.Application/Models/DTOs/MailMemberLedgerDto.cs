using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Models.DTOs
{
    public class MailMemberLedgerDto
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<string> MembershipNoList { get; set; }
        public string Subject { get; set; }
    }
}
