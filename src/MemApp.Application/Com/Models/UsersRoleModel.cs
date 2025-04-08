using MemApp.Application.Extensions;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Com.Models
{
    public class UsersRoleModel
    {
        
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsChecked { get; set; }

      
    }

    public class UsersRoleModelListVm : Result
    {
        public List<UsersRoleModel> DataList { get; set; } = new List<UsersRoleModel>();
    }

    public class UsersRoleModelReq
    {
        public int UserId { get; set; }
        public List<int> PermissionList { get; set; } = new List<int>();
    }
}
