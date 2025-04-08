using MemApp.Application.Extensions;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Colleges.Models
{
    public class CommCatModel
    {
    }
    public class CommCatReq
    {
        public string Title { get; set; } = string.Empty;
        public int Id { get; set; }
    }
    
    public class CommCatVm: Result
    {
       public CommCatReq Data { get; set; } = new CommCatReq();
    }
    public class CommCatListVm : Result
    {
        public long DataCount { get; set; }
        public List<CommCatReq> DataList { get; set; }= new List<CommCatReq>();
    }
}
