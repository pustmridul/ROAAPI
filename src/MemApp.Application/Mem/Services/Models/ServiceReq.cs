using MemApp.Application.Extensions;
using MemApp.Application.Mem.MemServices.Models;
using MemApp.Application.Mem.Service.Model;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Services.Models
{
    public class ServiceReq
    {
        public int Id { get; set; }
        public int ServiceTypeId { get; set; }
        public string Title { get; set; }

    }

    public class ServiceRes
    {
       
        public int Id { get; set; }
        public int ServiceTypeId { get; set; }
        public string ServiceTypeTitle { get; set; }
        public string Title { get; set; }
        
    }

    public class ServiceVm : Result
    {
        public ServiceRes Data { get; set; } = new ServiceRes();
    }

    public class ServiceListVm : Result
    {
        public long DataCount { get; set; }
        public List<ServiceRes> DataList { get; set; } = new List<ServiceRes>();

    }
}
