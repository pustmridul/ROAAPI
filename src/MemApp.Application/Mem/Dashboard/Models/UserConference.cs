using MemApp.Application.Models.Responses;
using MemApp.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MemApp.Application.Extensions;

namespace MemApp.Application.Mem.Dashboard.Models
{
    public class UserConferenceResp 
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime? LogInDate { get; set; }
        public DateTime? LogOutDate { get; set; }
        public bool LogOutStatus { get; set; }
        public string AppId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
       
       
    }

    public class UserConferenceVmList : Result
    {
        public int DataCount { get; set; }
        public List<UserConferenceResp> DataList { get; set; } = new List<UserConferenceResp>();
    }
}
