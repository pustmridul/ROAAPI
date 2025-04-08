using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class Attendance :BaseEntity
    {
        public string MemberShipNo { get; set; }
        public int MemberId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public bool IsActive { get; set; }

    }
}
