using MemApp.Application.Extensions;

namespace MemApp.Application.Mem.MiscItems.Models
{
    public class MiscItemReq
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Id { get; set; }
    }

    public class MiscItemVm : Result
    {
        public MiscItemReq Data { get; set; } = new MiscItemReq();
    }

    public class MiscItemListVm : ListResult<MiscItemReq>
    {
    }
}
