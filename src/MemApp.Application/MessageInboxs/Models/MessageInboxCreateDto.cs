using MemApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.MessageInboxs.Models
{
    public class MessageInboxCreateDto
    {
        public int Id { get; set; }
        public int? MemberId { get; set; }
        public string Message { get; set; }
        public MessageInboxTypeEnum TypeId { get; set; }
        public bool IsRead { get; set; }
        public bool IsAllMember { get; set; } = true;
    }
}
