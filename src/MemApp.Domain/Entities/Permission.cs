using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Res.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public string ModuleName { get; set; } = string.Empty;
        public string OperationName { get; set; } = string.Empty;
        public int PermissionNo { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
