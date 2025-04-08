using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.mem;

namespace MemApp.Domain.Entities.ser
{
    public class AvailabilityDetail : BaseEntity
    {
        public int AvailabilityId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Title { get; set; }
        public bool IsChecked { get; set; }
        public bool IsWholeDay { get; set; }
        public bool IsActive { get; set; }

        public Availability Availabilities { get; set; }

    }
}
