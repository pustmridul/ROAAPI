using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MemApp.Application.Mem.Notification
{
    public class PushNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

     

        public PushNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotification(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
