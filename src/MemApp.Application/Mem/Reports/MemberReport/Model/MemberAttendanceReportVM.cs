using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.MemberReport.Model
{
    public class MemberAttendanceReportVM
    {
        public int RegisterMemberId { get; set; }
        public string? Name { get; set; }
        public string? MembershipNo { get; set; }
        public DateTime? InTime { get; set; }
    }

}
