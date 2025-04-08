using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities
{
    public class RefreshToken 
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? AppId { get; set; }
        public int? UserId { get; set; }
        public string Token { get; set; }
        public string RefToken { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.Now >= Expires;
        public string CreatedByIp { get; set; }
       
    }
}