using MemApp.Application.App.Models;
using MemApp.Application.Extensions;
using ResApp.Application.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResApp.Application.App.Models
{
    public class ResMemberLoginDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<int> Roles { get; set; } = new List<int>();
        public bool IsVerified { get; set; }
        public string JWToken { get; set; } = string.Empty;
        public DateTime IssuedOn { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string RefreshToken { get; set; } = string.Empty;

        public MemberRegistrationInfoDto MemberInfo { get; set; } = new MemberRegistrationInfoDto();
    }

   


}
