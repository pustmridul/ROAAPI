using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Sales.Models
{
    public class AvailableServiceSlotListVm
    {
        public int Id { get; set; }
        public bool? IsWholeDay { get; set; }
        public bool IsActive { get; set; }
        public string StartTime { get; set; }= string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public int? ServiceTicketId { get; set; }
        public string? DayText { get; set; }
        public int Qty { get; set; }
        public int SoldQty { get; set; }
        public int SlotQty { get; set; }
        public bool IsSoldOut { get; set; }

    }
}
