using MemApp.Domain.Core.Models;
using MemApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.Communication
{
    public class AdminNotification: BaseEntity
    {
        public string Message { get; set; }
        public MessageInboxTypeEnum TypeId { get; set; }
        public bool IsRead { get; set; }
    }
}
