using MemApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Models.DTOs
{
    public class NotificationEmailDto
    {
        public string ServiceSaleNotificationEmail { get; set; }
        public string EventSaleNotificationEmail { get; set; }
        public string VenueBookingNotificationEmail { get; set; }
    }
}
