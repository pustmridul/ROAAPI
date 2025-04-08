using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities.com
{
    public class NotificationToken 
    {
        [Key]
        public int Id { get; set; } 
        public string DeviceToken { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
