using MemApp.Application.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Models.Requests
{
    

    public class RoleGroupReq
    {
        public int RoleId { get; set; }
        public int PermissionNo { get; set; }

    }
    public class RolePermissionRes : Result
    {
        public string Name { get; set; }=string.Empty;
        public int RoleId { get; set; }
        public List<PermissionVm> PermissionList { get; set; }= new List<PermissionVm>();
    }

    public class PermissionVm
    {
        public bool IsChecked { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PermissionNo { get; set; }
        public List<PermissionDetailVm> PermissionDetailVms { get; set; } = new List<PermissionDetailVm>();
    }
    public class PermissionDetailVm
    {
        public bool IsChecked { get; set; }
        public string Name { get; set; } = string.Empty;  
        public int PermissionNo { get; set; }
    }
}
