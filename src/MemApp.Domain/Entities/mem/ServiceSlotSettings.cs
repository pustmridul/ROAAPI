using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class ServiceSlotSettings : BaseEntity
    {
        public int SlotMasterId { get; set; }
        public string Day { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool IsWholeDay { get; set; }
        public bool IsActive { get; set; }
        public SlotMaster SlotMaster { get; set; }
    }
}
