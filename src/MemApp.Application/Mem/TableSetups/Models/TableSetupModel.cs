using MemApp.Application.Extensions;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.TableSetups.Models
{
    public class TableSetupModel
    {
    }
    public class TableSetupReq
    {
        public string Title { get; set; }
        public string? DisplayName { get; set; }
        public DateTime? ActiveDate { get; set; }
        public string? Status { get; set; }
        public bool IsActive { get; set; }
        public int Id { get; set; }
        
    }
    
   
    public class TableSetupVm: Result
    {
       public TableSetupReq Data { get; set; } = new TableSetupReq();
    }

    public class TableSetupListVm :Result
    {
        public long DataCount { get; set; }
        public List<TableSetupReq> DataList { get; set; }= new List<TableSetupReq>();
    }
}
