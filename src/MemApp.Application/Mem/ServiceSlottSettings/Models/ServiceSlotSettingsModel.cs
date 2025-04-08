using MemApp.Application.Extensions;
using MemApp.Application.Mem.AreaLayouts.Models;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.ServiceSlottSettings.Models
{
  
    public class Slot
    {
        public int Id { get; set; }
        public string StartTime { get; set; }= string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string DayText { get; set; } = string.Empty;
        public bool IsWholeDay { get; set; }
        public int Qty { get; set; }
        public bool IsChecked { get; set; }
        public List<Slot> SlotList { get; set; }= new List<Slot>();
    }
    public class ServiceSlotSettingsReq
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string? ServiceText { get; set; }
        public List<Slot> SlotList { get; set; } = new List<Slot>();
    }
    public class ServiceSlotSettingsVm : Result
    {
      public ServiceSlotSettingsReq Data { get; set; }= new ServiceSlotSettingsReq();
    }
    public class ServiceSlotSettingsListVm : ListResult<ServiceSlotSettingsReq>
    {

    }
}
