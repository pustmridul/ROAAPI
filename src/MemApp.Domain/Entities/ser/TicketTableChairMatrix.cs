using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.ser
{
    public class TicketTableChairMatrix
    {
        [Key]
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int MemServiceId { get; set; }
        public int AreaLayOutId { get; set; }
        public string? AreaLayOutText { get; set; }
        public int TableId { get; set; }
        public string? TableText { get; set; }
        public int ChairNo { get; set; }
        public string? ChairKeyNo { get; set; }
        public bool IsSale { get; set; }
        public bool IsRev { get; set; }
    }
}
