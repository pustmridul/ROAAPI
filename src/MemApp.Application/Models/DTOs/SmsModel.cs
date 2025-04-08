using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Models.DTOs
{
    public class EmailModel
    {
        public string? EmailSubject { get; set; }
        public string? Message { get; set; }
        public List<IFormFile>? Attachments { get; set; }
        public IFormFile? ExcelFile { get; set; }
        public List<string>? EmailList { get; set; }
    }
     public class CustomSmsReq
    {
        public string? EmailSubject { get; set; }
        public string? LanType { get; set; }
        public string? Message { get; set; }
        public string? PhoneNo { get; set; }
        public string? EmailId { get; set; }
    }

    public class BulkSmsReq
    {
        public string? LanType { get; set; }
        public string? EmailSubject { get; set; }

        public string? Message { get; set; }
        public List<SmsReq> SmsReqList { get; set; }= new List<SmsReq>();
    }
    public class SmsReq
    {
        public string? PhoneNo { get; set; }
        public string? EmailId { get; set; }

        public string? MemberName { get; set; }
        public string? MemberShipNo { get; set; }

    }
}
