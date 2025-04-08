using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities
{
    public class UserRoleMap : BaseEntity
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }

    }
}
