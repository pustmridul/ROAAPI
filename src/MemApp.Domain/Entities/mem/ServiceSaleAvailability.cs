using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class ServiceSaleAvailability : BaseEntity
    {
       
        public string? Title { get; set; }
        public int? AvailabilityId { get; set; }
        public int? ServiceSaleId { get; set; }
        public int? ServiceTicketId { get; set; }
        public int? ServiceAvailabilityDetailsId { get; set; }
        public bool? IsChecked { get; set; }
        public bool? IsWholeDay { get; set; }
        public bool IsActive { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? ReservationFrom { get; set; }
        public DateTime? ReservationTo { get; set; }
        public ServiceSale ServiceSale { get; set; }
    }
}
