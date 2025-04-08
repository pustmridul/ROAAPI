using MemApp.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.MessageInboxs.Models
{
    public class MessageInboxQueryDto
    {
        public int PageSize { get; set; }
        public int PageNo { get; set; }
        public int MemberId { get; set; }
        public bool? IsRead { get; set; }
    }

    public class TotalReadUnReadMessagesDto:Result
    {
        public int TotalMessageCount { get; set; }
    }
}
