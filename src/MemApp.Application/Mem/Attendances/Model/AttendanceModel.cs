using MemApp.Application.Extensions;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Attendances.Model
{
    public class AttendanceModel
    {
    }
    public class YearlyAttendance
    {
        public decimal ExpenseAmount { get; set; }
    }
    public class YearlyAttendanceList : Result
    {
        public List<int> YearlyAttendances { get; set; } = new List<int>();
    }
}
