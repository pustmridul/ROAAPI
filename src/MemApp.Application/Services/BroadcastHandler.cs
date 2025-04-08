using Azure;
using FirebaseAdmin.Messaging;
using MemApp.Application.App.Models;
using MemApp.Application.Extensions;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models.DTOs;
using MemApp.Domain.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RestSharp;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Xml.Serialization;

namespace MemApp.Application.Services
{
    public class BroadcastHandler : IBroadcastHandler
    {
        private readonly EmailSettings _emailSettings;
        private readonly SmsSettings _smsSettings;
        
        private readonly WhatsAppSettings _whatsAppSettings;
        private readonly SslCommerzSettings _sslCommerzSettings;

        //public string Single_Sms_Url = "https://smsplus.sslwireless.com/api/v3/send-sms";
        //public string Single_Sms_Sid = "CCCLNONOTP";
        //public string Single_Sms_csms_id = "1234569456456456456";
        //public string Single_Sms_api_token = "1hrcfczgy-llpr172d-trju9rhv-hbbvzap4-mgg1jjkb";


        private string SmsSendUrl = "https://smsplus.sslwireless.com/api/v3/send-sms?api_token={0}&sid={1}&sms={2}&msisdn={3}&csms_id={4}";

        private readonly IMemDbContext _context;
        public BroadcastHandler(
            IOptions<EmailSettings> emailSettings,
            IOptions<SmsSettings> smsSettings,
            IOptions<WhatsAppSettings> whatsAppSettings,
            IOptions<SslCommerzSettings> sslCommerzSettings,
            IMemDbContext context)
        {
            _emailSettings = emailSettings.Value;
            _smsSettings = smsSettings.Value;
            _whatsAppSettings = whatsAppSettings.Value;
            _sslCommerzSettings = sslCommerzSettings.Value;
            _context = context;
        }
        public async Task<bool> SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath)
        {
            try
            {

                using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                    EnableSsl = true,
                };
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(to);

                // Create an attachment
                var attachment = new Attachment(attachmentPath, MediaTypeNames.Application.Octet);
                mailMessage.Attachments.Add(attachment);

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("Email with attachment sent successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }

        }

        public async Task<bool> SendAttchmentEmail(int emailAccountId, string subject, string body, string toAddress, string toName, string replyTo = null, string replyToName = null, IEnumerable<string> bcc = null, IEnumerable<string> cc = null, string attachmentFilePath = null, string attachmentFileName = null, int attachedDownloadId = 0, IDictionary<string, string> headers = null)
        {
            if (_emailSettings.Enable == "1")
            {
                try
                {
                    using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                    {
                        Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                        EnableSsl = true,
                    };


                    var message = new MailMessage();
                    //from, to, reply to
                    message.From = new MailAddress("email", "Display Name");
                    message.To.Add(new MailAddress(toAddress, toName));
                    if (!string.IsNullOrEmpty(replyTo))
                    {
                        message.ReplyToList.Add(new MailAddress(replyTo, replyToName));
                    }

                    //BCC
                    if (bcc != null)
                    {
                        foreach (var address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
                        {
                            message.Bcc.Add(address.Trim());
                        }
                    }

                    //CC
                    if (cc != null)
                    {
                        foreach (var address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
                        {
                            message.CC.Add(address.Trim());
                        }
                    }

                    //content
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    //headers
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            message.Headers.Add(header.Key, header.Value);
                        }
                    }


                    if (!String.IsNullOrEmpty(attachmentFilePath) &&
                        File.Exists(attachmentFilePath))
                    {
                        var attachment = new Attachment(attachmentFilePath);
                        attachment.ContentDisposition.CreationDate = File.GetCreationTime(attachmentFilePath);
                        attachment.ContentDisposition.ModificationDate = File.GetLastWriteTime(attachmentFilePath);
                        attachment.ContentDisposition.ReadDate = File.GetLastAccessTime(attachmentFilePath);
                        if (!String.IsNullOrEmpty(attachmentFileName))
                        {
                            attachment.Name = attachmentFileName;
                        }
                        message.Attachments.Add(attachment);
                    }
                    await smtpClient.SendMailAsync(message);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else { return false; }

        }

        public async Task<bool> SendDefaultEmail(string to)
        {
            string subject = string.Empty;
            string body = string.Empty;
            return await SendEmail(to, subject, body);
        }

        public async Task<bool> SendEmail(string to, string subject, string body, string? memberName= null, string? memberShipNo = null)
        {
            if (_emailSettings.Enable == "1")
            {
                try
                {
                    using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                    {
                        Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                        EnableSsl = true,
                    };
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_emailSettings.SenderEmail),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(to);
                    await smtpClient.SendMailAsync(mailMessage);

                    EmailLog email = new EmailLog()
                    {
                        EmailId = to,
                        Message = body,
                        EmailDate = DateTime.Now,
                        MemberName = memberName != null ? memberName : "",
                        MemberShipNo = memberShipNo != null ? memberShipNo : "",
                        Status = "Ok"
                    };
                    await SaveEmailLog(email);


                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> SendSms(string mobileNo, string message, string lanType, string? memberName=null, string? memberShipNo=null)
        {

            var client = new RestClient(_smsSettings.SingleSmsUrl);
            var request = new RestRequest
            {
                Timeout = TimeSpan.FromMilliseconds(30000)  // Set timeout in milliseconds (e.g., 10 seconds)
            };
            request.Method = Method.Post;

            request.AddParameter("api_token", _smsSettings.SingleSmsApiToken);
            request.AddParameter("sid", _smsSettings.SingleSmsSid);
            request.AddParameter("msisdn", mobileNo);
            request.AddParameter("sms", message);
            request.AddParameter("csms_id", _smsSettings.SingleSmsCsmsId);
            RestResponse response = client.Execute(request);
           
            if (response.IsSuccessful)
            {
                SmsLog sms = new SmsLog()
                {
                    PhoneNo = mobileNo,
                    Message = message,
                    SmsDate = DateTime.Now,
                    MemberName = memberName != null ? memberName : "",
                    MemberShipNo = memberShipNo != null ? memberShipNo : "",
                    Status = response.IsSuccessful ? "Ok" : "No"
                };
                await SaveSmsLog(sms);
            }
            

            return response.IsSuccessful;

        }
        private string GetSmsSendUrl(string contacts, string msg)
        {
            string csms_id = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            string api_key = "95jzedqx-qy2ti9qp-hjegoocb-8uf8kr8b-zfutltho";
            string url = string.Format(SmsSendUrl, api_key, _sslCommerzSettings.EnglishSid, msg, contacts, csms_id);
            return url;
        }
        public Tuple<bool, string> SendSmsSslWireless(string type, string contacts, string msg)
        {
            string response_string = "Unknown Error";
            bool done = false;

            string apiUrl = GetSmsSendUrl(contacts, msg);

            using (var client = new HttpClient())
            {
                using (var response = client.GetAsync(apiUrl))
                {
                    if (response.Result.IsSuccessStatusCode)
                    {
                        string jsonString = response.Result.Content.ReadAsStringAsync().Result;

                        ApiResponse Result = JsonConvert.DeserializeObject<ApiResponse>(jsonString);

                        if (Result != null)
                        {
                            if (Result.status.ToUpper().Equals("SUCCESS"))
                            {
                                done = true;
                                response_string = Result.status;
                            }
                            else
                            {
                                done = false;
                                response_string = Result.error_message;
                            }
                        }
                        else
                        {
                            done = false;
                            response_string = "SMS Sending Failed";
                        }
                    }
                }
            }
            Tuple<bool, string> responseTuple = new Tuple<bool, string>(done, response_string);
            return responseTuple;
        }

        public async Task<bool> SendMessage(string mobile, string language, string template, List<WhatsAppComponent>? components = null)
        {
            using HttpClient httpClient = new();

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _whatsAppSettings.Token);

            WhatsAppRequest body = new()
            {
                to = mobile,
                template = new Template
                {
                    name = template,
                    language = new Language { code = language }
                }
            };

            if (components is not null)
                body.template.components = components;

            HttpResponseMessage response =
                await httpClient.PostAsJsonAsync(new Uri(_whatsAppSettings.ApiUrl), body);

            return response.IsSuccessStatusCode;
        }





        public async Task<bool> SaveSmsLog(SmsLog smsLog)
        {
            try
            {
                var obj = new SmsLog()
                {

                };
                obj = smsLog;
                _context.SmsLogs.Add(obj);
                await _context.SaveAsync();
            }
            catch (Exception ex)
            {

            }
            return true;
        }

        public async Task<bool> SaveEmailLog(EmailLog emailLog)
        {
            try
            {
                var obj = new EmailLog()
                {

                };
                obj = emailLog;
                _context.EmailLogs.Add(obj);
                await _context.SaveAsync();
            }
            catch (Exception ex)
            {

            }
            return true;
        }

        public async Task<PaginatedResult<SmsLog>> GetSmsLogs(int pageNo, int pageSize, string? searchText)
        {
            var data = await _context.SmsLogs.OrderByDescending(i => i.Id).ToPaginatedListAsync(pageNo, pageSize, new CancellationToken());
            return data;
        }

        public async Task<bool> SendEmailByAttachment(EmailModel model)
        {
            if (model.EmailList == null || !model.EmailList.Any())
                return false;

            using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                EnableSsl = true,
            };

            foreach (var email in model.EmailList)
            {

                var mail = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail),
                    Subject = model.EmailSubject,
                    Body = model.Message,
                    IsBodyHtml = true,
                };

                mail.To.Add(email);

                // Handle file attachments
                if (model.Attachments != null && model.Attachments.Any())
                {
                    foreach (var attachment in model.Attachments)
                    {
                        if (attachment.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                await attachment.CopyToAsync(ms);
                                var fileBytes = ms.ToArray();
                                mail.Attachments.Add(new Attachment(new MemoryStream(fileBytes), attachment.FileName));
                            }
                        }
                    }
                }

                try
                {
                    await smtpClient.SendMailAsync(mail);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return true;
        }
    }


    public class ApiResponse
    {
        public string status { get; set; }
        public int status_code { get; set; }
        public string error_message { get; set; }


    }

    [XmlRoot(ElementName = "SMSINFO")]
    public class SMSINFO
    {
        [XmlElement(ElementName = "MSISDN")]
        public string MSISDN { get; set; }
        [XmlElement(ElementName = "SMSTEXT")]
        public string SMSTEXT { get; set; }
        [XmlElement(ElementName = "CSMSID")]
        public string CSMSID { get; set; }
        [XmlElement(ElementName = "REFERENCEID")]
        public string REFERENCEID { get; set; }
    }

    [XmlRoot(ElementName = "REPLY")]
    public class REPLY
    {
        [XmlElement(ElementName = "PARAMETER")]
        public string PARAMETER { get; set; }
        [XmlElement(ElementName = "LOGIN")]
        public string LOGIN { get; set; }
        [XmlElement(ElementName = "PUSHAPI")]
        public string PUSHAPI { get; set; }
        [XmlElement(ElementName = "STAKEHOLDERID")]
        public string STAKEHOLDERID { get; set; }
        [XmlElement(ElementName = "PERMITTED")]
        public string PERMITTED { get; set; }
        [XmlElement(ElementName = "SMSINFO")]
        public SMSINFO SMSINFO { get; set; }
    }
}
