using Microsoft.Extensions.DependencyInjection;
using MemApp.Application.Interfaces;
using MemApp.Application.Services;
using System.Reflection;
using MediatR;
using MemApp.Application.Exceptions;
using MemApp.Application.Helper;
using ResApp.Application.Interfaces;
using ResApp.Application.Services;

namespace MemApp.Application.DependencyResolver
{
    public static class DependencyResolverService
    {
        public static IServiceCollection ApplicationRegister(this IServiceCollection services)
        {
            services.AddScoped<IPasswordHash, PasswordHash>();
            services.AddScoped<IPasswordNewHash, PasswordNewHash>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IDashboardService, DasboardService>();
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

            });
            services.AddAutoMapper(typeof(AutomapperProfile).Assembly);
            services.AddScoped<IMenuService, MenuService>();
            services.AddAutoMapper(typeof(AutomapperProfile).Assembly);
            services.AddTransient<IPermissionHandler, PermissionHandler>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IFileSaveService, FileSaveService>();
            services.AddTransient<IMemLedgerService, MemLedgerService>();
            services.AddScoped<IUserLogService, UserLogService>();
            services.AddScoped<IBroadcastHandler,  BroadcastHandler>();

            services.AddScoped<INotificationService, NotificationService>();
         

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            return services;
        }
    }
}