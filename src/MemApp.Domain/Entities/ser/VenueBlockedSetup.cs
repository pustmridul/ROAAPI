using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.ser
{
    public class VenueBlockedSetup : BaseEntity
    {
        public DateTime BlockedDate { get; set; }
        public bool IsActive { get; set; }
        public string DayName { get; set; }
        public string MonthName { get; set; }
        public string YearName { get; set; }
        public int VenueId { get; set; }
        public string? VenueTitle { get; set; }
        public int AvailabilityId { get; set; }
    }
}
