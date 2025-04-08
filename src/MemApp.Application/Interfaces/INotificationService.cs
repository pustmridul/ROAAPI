using MemApp.Application.Models.DTOs;
using MemApp.Domain.Entities.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendPushNotificationAsync(NotificationModel notificationModel);
        Task<List<AdminNotification>> GetAllNotification();
        Task<List<AdminNotification>> GetUnreadNotification();
        Task<NotificationEmail> GetNotificationEmail();
        Task<bool> UpdateNotificationStatus(int[] unreadNotificationIds);
        Task<bool> SaveOrUpdateNotificationEmail(NotificationEmailDto notificationEmail);
    }
}
