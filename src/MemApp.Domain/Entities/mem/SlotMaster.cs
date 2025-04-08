using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem;

public class SlotMaster : BaseEntity
{
    public int ServiceId { get; set; }
    public string? ServiceText { get; set; }
    public ICollection<ServiceSlotSettings> ServiceSlots { get; set; }
}
