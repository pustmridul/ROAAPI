using MemApp.Application.Mem.Notification;
using MemApp.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace MemApp.WebApi
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddWebUIServices(this IServiceCollection services)
        {
            services.AddHealthChecks()
               .AddDbContextCheck<MemDbContext>();


            services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);
            services.AddSingleton<PushNotificationService>();


            return services;

        }
    }
}
