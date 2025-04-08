using MemApp.Domain.Core.Models;
using MemApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem;

public class MessageInbox : BaseEntity
{
    public int MemberId { get; set; }
    public string Title { get; set; }   
    public string Message { get; set; }
    public MessageInboxTypeEnum TypeId { get; set; }
    public bool IsRead { get; set; }
}
