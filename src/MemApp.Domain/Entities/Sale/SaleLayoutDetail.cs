using MemApp.Domain.Core.Models;
using MemApp.Domain.Entities.ser;

namespace MemApp.Domain.Entities.Sale
{
    public class SaleLayoutDetail : BaseEntity
    {
        public SaleMaster SaleMaster { get; set; }
        public int SaleMasterId { get; set; }
        public int AreaLayoutId { get; set; }
        public string? AreaLayoutTitle { get; set; }
        public int TableId { get; set; }
        public string? TableName { get; set; }
        public int NoofChair { get; set; }

    }
}
