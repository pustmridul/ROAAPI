using MemApp.Application.Interfaces;
using MemApp.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemApp.Infrastructure.Services
{
    public class DailyEmailService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;
     


        public DailyEmailService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Calculate the time until midnight
            var now = DateTime.Now;
            var midnight = DateTime.Today.AddDays(1); // Next midnight
            var initialDelay = midnight - now; // Time remaining until midnight

            // Set up the timer to run at midnight, then every 24 hours
            _timer = new Timer(SendDailyEmails, null, initialDelay, TimeSpan.FromDays(1));

            return Task.CompletedTask;

        }

        //public Task StartAsync(CancellationToken cancellationToken)
        //{
        //    // Calculate the time until 12:55 PM
        //    var now = DateTime.Now;
        //    var targetTime = DateTime.Today.AddHours(14).AddMinutes(22); // Today's 12:55 PM

        //    // If the target time has already passed today, schedule for tomorrow
        //    if (now > targetTime)
        //    {
        //        targetTime = targetTime.AddDays(1); // Next day's 12:55 PM
        //    }

        //    var initialDelay = targetTime - now; // Time remaining until 12:55 PM

        //    // Set up the timer to run at 12:55 PM, then every 24 hours
        //    _timer = new Timer(SendDailyEmails, null, initialDelay, TimeSpan.FromDays(1));

        //    return Task.CompletedTask;
        //}


        private async void SendDailyEmails(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _broadcastHandler = scope.ServiceProvider.GetRequiredService<IBroadcastHandler>();


                var today = DateTime.Today;
           

                var memberList = await scope.ServiceProvider.GetRequiredService<ICacheDataLoadHelper>().GetAllMember();
                
                var usersWithBirthdayToday = memberList
                               .Where(u => u.Dbo.HasValue && u.Dbo.Value.Month == today.Month && u.Dbo.Value.Day == today.Day && u.IsMasterMember == true)
                               .ToList();
                var usersWithAnnyversaryToday = memberList
                             .Where(u => u.Anniversary.HasValue && u.Anniversary.Value.Month == today.Month && u.Anniversary.Value.Day == today.Day && u.IsMasterMember == true)
                             .ToList();
                var templates = await scope.ServiceProvider.GetRequiredService<ICacheDataLoadHelper>().GetAllMessageTemplate();

                foreach (var user in usersWithBirthdayToday)
                {
                    if (user == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (user.Email == null)
                        {
                            continue;
                        }
                        else
                        {

                            var smsTemplate = templates.Where(c => c.MessageTypeId == (int)MessageTypeEnum.SMS && c.OccasionTypeId == (int)OccasionType.Birthday).FirstOrDefault();
                            var emailTemplate = templates.Where(c => c.MessageTypeId == (int)MessageTypeEnum.Email && c.OccasionTypeId == (int)OccasionType.Birthday).FirstOrDefault();

                            if (smsTemplate != null)
                            {
                                var message = smsTemplate?.Message?.Replace("[membershipno]", user.MemberShipNo).Replace("[membername]", user.Name);

                               _broadcastHandler.SendSms(user.Phone, message, "English", user.Name,user.MemberShipNo).Wait();
                            }
                            if (emailTemplate != null)
                            {
                                var message = emailTemplate?.Message?.Replace("[membershipno]", user.MemberShipNo).Replace("[membername]", user.Name);
                                _broadcastHandler.SendEmail(user.Email, emailTemplate.Subject, message, user.Name, user.MemberShipNo).Wait();
                            }
                        }
                    }

                }

                foreach (var user in usersWithAnnyversaryToday)
                {
                    if (user == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (user.Email == null)
                        {
                            continue;
                        }
                        else
                        {

                            var smsTemplate = templates.Where(c => c.MessageTypeId == (int)MessageTypeEnum.SMS && c.OccasionTypeId == (int)OccasionType.Anniversary).FirstOrDefault();
                            var emailTemplate = templates.Where(c => c.MessageTypeId == (int)MessageTypeEnum.Email && c.OccasionTypeId == (int)OccasionType.Anniversary).FirstOrDefault();

                            if (smsTemplate != null)
                            {
                                var message = smsTemplate?.Message?.Replace("[membershipno]", user.MemberShipNo).Replace("[membername]", user.Name);

                                _broadcastHandler.SendSms(user.Phone, message, "English", user.Name, user.MemberShipNo).Wait();
                            }
                            if (emailTemplate != null)
                            {
                                var message = emailTemplate?.Message?.Replace("[membershipno]", user.MemberShipNo).Replace("[membername]", user.Name);
                                _broadcastHandler.SendEmail(user.Email, emailTemplate.Subject, message, user.Name, user.MemberShipNo).Wait();
                            }
                        }
                    }

                }

            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
