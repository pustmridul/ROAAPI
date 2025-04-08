using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.ser
{
    public class SerTicketAreaLayout : BaseEntity
    {
        public int ServiceTicketId { get; set; }
        public int AreaLayoutId { get; set; }
        public string? AreaLayoutTitle { get; set; }

        public ServiceTicket ServiceTicket { get; set; }

        public ICollection<SerTicketAreaLayoutMatrix> SerTicketAreaLayoutMatrices { get; set; }

    }
}
