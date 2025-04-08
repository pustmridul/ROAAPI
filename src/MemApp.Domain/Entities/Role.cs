using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public ICollection<UserRoleMap> UserRoles { get; set; }
    }
}
