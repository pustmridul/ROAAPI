using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Models.DTOs
{
    public class VenueBookingMessage
    {
        public decimal Amount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal ServiceAmount { get; set; }
        public string Phone { get; set; }
    }
}
