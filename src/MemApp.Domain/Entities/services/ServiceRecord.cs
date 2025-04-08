using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.services
{
    public class ServiceRecord : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ServiceTypeId { get; set; }
        public bool IsActive { get; set; } = true;
        public ServiceType ServiceType { get; set; }
    }
}
