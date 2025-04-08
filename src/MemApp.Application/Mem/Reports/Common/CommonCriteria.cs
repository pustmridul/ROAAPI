using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Reports.Common
{
    public class CommonCriteria
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? EventId { get; set; }
        public string UserName { get; set; }
        public int? ServiceId { get; set; }
        public int? VenueId { get; set; }
        public string? MembershipNo { get; set; } = null;
        public int? PageSize { get; set;}
        public int? PageNo { get; set; }


    }
}
