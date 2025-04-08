using MemApp.Application.Extensions;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.AreaLayouts.Models
{
    public class AreaLayoutModel
    {
    }
    public class AreaLayoutReq
    {
        public string Title { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public DateTime? ActiveDate { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
        public int Id { get; set; }
        public List<AreaLayoutDetailReq> AreaLayoutDetails { get; set; }= new List<AreaLayoutDetailReq>();
    }
    public class AreaLayoutDetailReq
    {
        public int Id { get; set; }
        public string TableName { get; set; }= string.Empty;
        public int TableId { get; set; }
        public int? NumberOfChair { get; set; }
        public int AreaLayoutId { get; set; }
        public bool? IsActive { get; set; }
    }
    public class AreaLayoutTableDetailVm: Result
    {
        public AreaLayoutTableDetail Data { get; set; } = new AreaLayoutTableDetail();
    }
  
    public class AreaLayoutTableDetail
    {
        public int AreaLayoutId { get; set; }
        public string Title { get; set; }= string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public List<TableDetailVm> TableListVm { get; set; }= new List<TableDetailVm>();
    }

     public class TableDetailVm
    {
        public int Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public List<ChairVm> ChairList { get; set; }= new List<ChairVm>();

    }
    public class ChairVm
    {
        public int ChairNo { get; set; }
        public string? ChairKeyNo { get; set; }
        public bool IsSale { get; set; }

    }
    public class AreaLayoutVm: Result
    {
       public AreaLayoutReq Data { get; set; } = new AreaLayoutReq();
    }

    public class AreaLayoutListVm : Result
    {
        public long DataCount { get; set; }
        public List<AreaLayoutReq> DataList { get; set; }= new List<AreaLayoutReq>();
    }
}
