using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Models
{
    [Serializable]
    public class UserProfile
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string UserName { get; set; }
        public string? EmailId { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? PhoneNo { get; set; }
        public string AppId { get; set; }
        public int? MemberId { get; set; }
        public List<int>? Roles { get; set; }
    }
}
