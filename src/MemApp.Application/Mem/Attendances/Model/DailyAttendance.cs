using MemApp.Application.Extensions;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Attendances.Model
{
    public class DailyAttendance: Result
    {
        public string MembershipNo { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string InTime { get; set; } = string.Empty;
        public string OutTime { get; set; } = string.Empty;
        public int DataCount { get; set; }

    }
    public class DailyAttendanceList : Result
    {
        public List<DailyAttendance> DataList { get; set; }= new List<DailyAttendance>();
        public int DataCount { get; set; }

    }



}
