using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.com
{
    public class RolePermissionMap : BaseEntity
    {
        public int RoleId { get; set; }
        public int PermissionNo { get; set; }
        public bool IsActive { get; set; }
    }
}
