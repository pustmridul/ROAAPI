using FirebaseAdmin.Messaging;
using MemApp.Application.Interfaces;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Models.DTOs;
using MemApp.Domain.Entities.Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace MemApp.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMemDbContext _memDbContext;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public NotificationService(IMemDbContext memDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _memDbContext = memDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendPushNotificationAsync(NotificationModel model)
        {
            List<string> registrationTokens = new List<string>();
            var tokenList = await _memDbContext.NotificationTokens
                .Where(q => !(string.IsNullOrEmpty(q.DeviceToken) || q.DeviceToken.ToLower() == "null"))
                .Select(s => s.DeviceToken)
                .Distinct()
                .ToListAsync();

            foreach (var token in tokenList)
            {
                if (!string.IsNullOrEmpty(token))
                {
                    registrationTokens.Add(token);
                }
            }

            ////List<string> tokens= new List<string>();
            ////tokens.Add("dKSk0A3wR46GbV2AbwatoE:APA91bHySpABBRYKoqLbULbSNPXZM5aigQEvd6axzwp06XEzY7i7bzsYKMTIHG4X3MFP86ywDR5ojPK-o3vBthAQSFv_T6t1NzB_HTQjeVt6wuKoTCH5N2k");

            // Send notifications in batches of 100
            const int batchSize = 100;
            for (int i = 0; i < registrationTokens.Count; i += batchSize)
            {
                // Take a batch of up to 100 tokens
                var tokenBatch = registrationTokens.Skip(i).Take(batchSize).ToList();

                var message = new MulticastMessage()
                {
                    Tokens = tokenBatch,
                    Notification = new Notification
                    {
                        Title = model.Title,
                        Body = model.Body,
                    },
                };

                try
                {
                    var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
                }
                catch (Exception ex)
                {
                    // Log the error or handle it appropriately
                    throw new Exception($"Failed to send notification: {ex.Message}");
                }
            }
        }

        public async Task<List<AdminNotification>> GetAllNotification()
        {
            return await _memDbContext.AdminNotifications.OrderByDescending(c => c.Id).ToListAsync();
        }

        public async Task<List<AdminNotification>> GetUnreadNotification()
        {
            return await _memDbContext.AdminNotifications
                .Where(c => c.IsRead == false)
                .OrderByDescending(c => c.Id).ToListAsync();
        }

        public async Task<NotificationEmail> GetNotificationEmail()
        {
            var result = await _memDbContext.NotificationEmails.FirstOrDefaultAsync();
            return result;
        }

        public async Task<bool> UpdateNotificationStatus(int[] unreadNotificationIds)
        {
            var cancellationToken = _httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;

            var notifications = await _memDbContext.AdminNotifications
                 .Where(n => unreadNotificationIds.Contains(n.Id))
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }


            if (await _memDbContext.SaveChangesAsync(cancellationToken) > 0)
            {
                return true;
            }
            return false;

        }

        public async Task<bool> SaveOrUpdateNotificationEmail(NotificationEmailDto notificationEmailsDto)
        {
            var cancellationToken = _httpContextAccessor.HttpContext?.RequestAborted ?? CancellationToken.None;
            // Check if any record exists (since it's keyless, we use FirstOrDefault or similar)
            var existingNotificationEmail = await _memDbContext.NotificationEmails.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

            if (existingNotificationEmail != null)
            {
                // Update existing record manually
                existingNotificationEmail.ServiceSaleNotificationEmail = notificationEmailsDto.ServiceSaleNotificationEmail;
                existingNotificationEmail.EventSaleNotificationEmail = notificationEmailsDto.EventSaleNotificationEmail;
                existingNotificationEmail.VenueBookingNotificationEmail = notificationEmailsDto.VenueBookingNotificationEmail;

                // Mark entity as modified for update
                _memDbContext.NotificationEmails.Update(existingNotificationEmail);
            }
            else
            {
                // Add a new record if no existing record is found
                var newNotificationEmail = new NotificationEmail
                {
                    ServiceSaleNotificationEmail = notificationEmailsDto.ServiceSaleNotificationEmail,
                    EventSaleNotificationEmail = notificationEmailsDto.EventSaleNotificationEmail,
                    VenueBookingNotificationEmail = notificationEmailsDto.VenueBookingNotificationEmail
                };

                await _memDbContext.NotificationEmails.AddAsync(newNotificationEmail, cancellationToken);
            }

            // Save changes
            await _memDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }



    }
}
