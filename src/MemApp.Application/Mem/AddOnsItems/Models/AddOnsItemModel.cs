using MemApp.Application.Extensions;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.AddOnsItems.Models
{
    public class AddOnsItemModel
    {
    }
    public class AddOnsItemReq
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PriceDate { get; set; } = string.Empty;
        public string? Description { get; set; }

    }
    public class AddOnsItemVm:Result
    {
       public AddOnsItemReq Data { get; set; } = new AddOnsItemReq();
    }

    public class AddOnsItemListVm :Result
    {
        public long DataCount { get; set; }
        public List<AddOnsItemReq> DataList { get; set; }= new List<AddOnsItemReq>();
    }

    public class AddOnsSearchReq
    {
        public int PageNo { get; set; }
        public int PageSize{ get; set; }
        public string? SearchText { get; set; }

    }
}
