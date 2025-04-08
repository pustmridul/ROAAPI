using MemApp.Application.Extensions;
using MemApp.Application.Mem.Booking.Models;
using MemApp.Domain.Entities.mem;
using MemApp.Domain.Entities.Restaurant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Restaurant.Reports.Stock.Model
{
    public class RawMaterialGroupListVM : Result
    {
        public List<RawMeterialGroup> DataList { get; set; } = new List<RawMeterialGroup>();
        public long DataCount { get; set; }
    }
}
