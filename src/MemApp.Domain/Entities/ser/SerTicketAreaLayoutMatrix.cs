using MemApp.Domain.Core.Models;

namespace MemApp.Domain.Entities.ser
{
    public class SerTicketAreaLayoutMatrix : BaseEntity
    {
        public int SerTicketAreaLayoutId { get; set; }
        public int TableId { get; set; }
        public string? TableTitle { get; set; }
        public int NoofChair { get; set; }
        public SerTicketAreaLayout SerTicketAreaLayout { get; set; }

    }
}
