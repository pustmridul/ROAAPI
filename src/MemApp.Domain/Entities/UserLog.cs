using MemApp.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Domain.Entities
{
    public class UserLog : BaseEntity
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime LogDate { get; set; }
        public string LogText { get; set; }
    }
}
