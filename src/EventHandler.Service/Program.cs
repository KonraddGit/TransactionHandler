using System;

namespace EventHandler.Service
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            var serviceName = AssemblyInformation.SoftwareName;
            var serviceVersion = AssemblyInformation.SoftwareVersion;

            try
            {
                logger.Info("Starting {ServiceName} v.{ServiceVersion}...", serviceName, serviceVersion);
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                logger.Error($"{serviceName} stopped because of an exception. {exception}");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
                    => Host.CreateDefaultBuilder(args)
                    .UseWindowsService()
                    .ConfigureServices((_, services) => services
                        .AddHostedService<EventHandlerHostedService>()
                        .RegisterServices())
                    .ConfigureLogging(logging => logging
                        .ClearProviders()
                        .AddConsole()
                        .SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace))
                    .UseNLog();
    }
}
