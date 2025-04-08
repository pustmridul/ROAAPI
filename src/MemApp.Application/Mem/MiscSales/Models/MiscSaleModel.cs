using MemApp.Application.Extensions;

namespace MemApp.Application.Mem.MiscSales.Models
{
    public class MiscSaleReq
    {
        public int Id { get; set; }
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public int? MemberId { get; set; }
        public string? MemberText { get; set; }
        public string? MemberShipNo { get; set; }

        public string? Note { get; set; }

        public List<MiscSaleDetailReq> MiscSaleDetailReqs { get; set; } = new List<MiscSaleDetailReq>();
    }

    public class MiscSaleDetailReq
    {
        public int Id { get; set; }
        public int MiscSaleId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string? ItemText { get; set; }
        public decimal UnitPrice { get; set; }
    }
    public class MiscSaleSearchModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }

    }
    public class MiscTemplateSearchModel
    {
        public int? MessageTypeId { get; set; }
        public int? OccasionTypeId { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? SearchText { get; set; }

    }
    public class MiscSaleVm : Result
    {
        public MiscSaleReq Data { get; set; } = new MiscSaleReq();
    }

    public class MiscSaleListVm : ListResult<MiscSaleReq>
    {
    }

}
