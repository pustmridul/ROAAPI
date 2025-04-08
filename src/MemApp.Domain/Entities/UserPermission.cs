using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities
{
    public class UserPermission : BaseEntity
    {
        public int UserId { get; set; }
        public int PermissionNo { get; set; }
    }
}
