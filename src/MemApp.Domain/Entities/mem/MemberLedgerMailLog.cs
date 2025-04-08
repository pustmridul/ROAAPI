using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.mem
{
    public class MemberLedgerMailLog
    {
        [Key]
        public int Id { get; set; }
        public string? Message { get; set; }
        public string? Level { get; set; }
        public DateTime? TimeStamp { get; set; }

    }
}
