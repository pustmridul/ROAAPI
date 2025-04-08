using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Com.Models
{
    public class PermissionModel
    {
        public int UserId { get; set; }
        public int? ParentID  {  get;  set; }

        public string Name {  get; set; }

        public int ID { get; set; }

        public List<PermissionModel> Childs { get; set; }= new List<PermissionModel>();

        public bool IsChecked { get; set; }

        public bool IsExpanded { get; set; }
    }

    public class PermissionModelListVm
    {
        public List<PermissionModel> PermissionList { get; set; } = new List<PermissionModel>();
    }

    public class UserPermissionReq
    {
        public int UserId { get; set; }
        public List<int> PermissionList { get; set; } = new List<int>();
    }
}
