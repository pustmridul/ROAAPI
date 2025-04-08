using MemApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.Communication
{
    
    [Table("mem_NotificationEmail")]
    public class NotificationEmail
    {
        [Key]
        public int Id { get; set; }
        public string ServiceSaleNotificationEmail { get; set; }
        public string EventSaleNotificationEmail { get; set; }
        public string VenueBookingNotificationEmail { get; set; }
    }
}
