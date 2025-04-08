using MemApp.Application.Extensions;
using MemApp.Application.Models.DTOs;
using MemApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Interfaces
{
    public interface IBroadcastHandler
    {
       // Tuple<bool, string> SendSmsSslWireless(string type, string contacts, string msg);
        Task<bool> SendSms(string mobileNo, string message, string langType, string? memberName=null, string? memberShipNo = null);
       // bool SendSms(string mobileNo, string message, string langType);

        Task<bool> SendDefaultEmail(string to);
        Task<bool> SendEmail(string to, string subject, string body, string? memberName = null, string? memberShipNo = null);
        Task<bool> SendAttchmentEmail(int emailAccountId, string subject, string body,
            string toAddress, string toName,
            string replyTo = null, string replyToName = null,
            IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            string attachmentFilePath = null, string attachmentFileName = null,
            int attachedDownloadId = 0, IDictionary<string, string> headers = null);

        Task<bool> SendMessage(string mobile, string language, string template, List<WhatsAppComponent>? components = null);
        Task<bool> SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath);
        Task<bool> SaveSmsLog(SmsLog model);
        Task<PaginatedResult<SmsLog>> GetSmsLogs(int pageNo, int pageSize, string? searchText);

        Task<bool> SendEmailByAttachment(EmailModel model);
    }
}
