using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.AppDownloadReport.Model
{
    public class AppDownloadReportVM
    {
        public bool IsMasterMember { get; set; }
        public string? MembershipNo { get; set; }
        public string? MembershipName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsActiveAppUser { get; set; }


    }
}
