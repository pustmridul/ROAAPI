using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MemApp.Application.Core.Repositories;
using MemApp.Application.Core.Services;
using MemApp.Infrastructure.Repositories;
using MemApp.Infrastructure.Services;
using MemApp.Application.Interfaces.Contexts;
using MemApp.Application.Services;

using MemApp.Application.Helper;
using MemApp.Application.Interfaces;
using MemApp.Infrastructure.CacheData;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MemApp.Infrastructure.DependencyResolver
{
    public static class DependencyResolverService
    {
        public static void InfrastructurRegister(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddSingleton<IDapperContext, DapperContext>();
            services.AddDbContext<MemDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("ApplicationConnection"),
            b => b.MigrationsAssembly(typeof(MemDbContext).Assembly.FullName)));
            services.AddAutoMapper(typeof(AutomapperProfile).Assembly);
            services.AddScoped(typeof(IBaseRepositoryAsync<>), typeof(BaseRepositoryAsync<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ILoggerService, LoggerService>();
            services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

            services.AddScoped<IMemDbContext, MemDbContext>();
            services.AddScoped<ICacheDataLoadHelper, CacheDataLoadHelper>();
           // services.AddSingleton<ISubscription, SubscriptionImp>();
            services.AddSingleton<IROASubscription, RoaSubscription>();

        }
    }
}