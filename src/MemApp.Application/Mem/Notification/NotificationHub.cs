using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Application.Mem.Notification
{
    public class NotificationHub:Hub
    {
        public async Task NewMessage(long username, string message) =>
        await Clients.All.SendAsync("messageReceived", username, message);
    }
}
