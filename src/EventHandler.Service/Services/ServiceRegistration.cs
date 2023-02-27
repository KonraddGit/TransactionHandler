using EventHandler.Application.Configuration;
using EventHandler.Application.Contracts.Application;
using EventHandler.Application.Contracts.Infrastructure.ControlCenterService;
using EventHandler.Application.Contracts.Infrastructure.Factory;
using EventHandler.Application.Contracts.Infrastructure.Persistence;
using EventHandler.Infrastructure.ControlCenterService;
using EventHandler.Infrastructure.Factory;
using EventHandler.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace EventHandler.Service.Services
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services
                .AddScoped<ISocketListenerFactory, SocketListenerFactory>()
                .AddScoped<IMessageRepository, MessageRepository>();

            services
                .AddSingleton<IFileConfigurationProvider, FileConfigurationProvider>()
                .AddSingleton<IDatabaseConfigurationProvider, DatabaseConfigurationProvider>()
                .AddSingleton<IConfigurationFactory, ConfigurationFactory>()
                .AddSingleton<IBackupManager, FileBackupManager>()
                .AddSingleton<IConfigurationRepository, ConfigurationRepository>()
                .AddSingleton<IControlCenterServiceAdapter, ControlCenterServiceAdapter>()
                .AddSingleton<IControlCenterEventDispatcher, ControlCenterEventDispatcher>()
                .AddSingleton<IControlCenterServiceConnector, ControlCenterServiceConnector>()
                .AddSingleton<IControlCenterServiceOperationsHandler, ControlCenterServiceOperationsHandler>();

            services.AddHttpClient();

            return services;
        }
    }
}
