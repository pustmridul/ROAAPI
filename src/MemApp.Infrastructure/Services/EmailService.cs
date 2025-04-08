using MemApp.Application.Core.Models;
using MemApp.Application.Core.Services;
using System.Net;
using System.Net.Mail;
using MemApp.Application.Models.DTOs;
using Microsoft.Extensions.Options;

namespace MemApp.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            
            _emailSettings = emailSettings.Value;
        }

        public void SendEmail(Email email)
        {
            try
            {
                // from and to addresses are required
                if (string.IsNullOrWhiteSpace(email.From) || string.IsNullOrWhiteSpace(email.To))
                {
                    return;
                }

                using (var smtpClient = new SmtpClient())
                {
                    // SMTP configuration from appsetting.json file
                    var deliveryMethod = _emailSettings.DeliveryMethod;
                    if (deliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory.ToString())
                    {
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                        smtpClient.PickupDirectoryLocation = _emailSettings.PickupDirectoryLocation;
                    }
                    else if (deliveryMethod == SmtpDeliveryMethod.Network.ToString())
                    {
                        // SMTP server
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.Host = _emailSettings.SmtpServer;
                        smtpClient.Port = Convert.ToInt32(_emailSettings.SmtpPort);
                        smtpClient.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                        smtpClient.EnableSsl = Convert.ToBoolean(_emailSettings.EnableSsl);
                    }

                    using (var mailMessage = new MailMessage())
                    {
                        // From
                        mailMessage.From = new MailAddress(email.From);

                        // Tos
                        var toMailAddresses = email.To.Split(';');
                        foreach (var mailAddress in toMailAddresses)
                        {
                            mailMessage.To.Add(mailAddress);
                        }

                        // CCs
                        if (!string.IsNullOrWhiteSpace(email.Cc))
                        {
                            var ccMailAddresses = email.Cc.Split(';');
                            foreach (var mailAddress in ccMailAddresses)
                            {
                                mailMessage.CC.Add(mailAddress);
                            }
                        }

                        // BCCs
                        if (!string.IsNullOrWhiteSpace(email.Bcc))
                        {
                            var bccMailAddresses = email.Bcc.Split(';');
                            foreach (var mailAddress in bccMailAddresses)
                            {
                                mailMessage.Bcc.Add(mailAddress);
                            }
                        }

                        mailMessage.Subject = email.Subject;
                        mailMessage.Body = email.Body;
                        mailMessage.IsBodyHtml = true;

                        // Attachments
                        if (email.Attachments != null && email.Attachments.Count > 0)
                        {
                            foreach (var attchment in email.Attachments)
                            {
                                mailMessage.Attachments.Add(attchment.File);
                            }
                        }

                        smtpClient.Send(mailMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}