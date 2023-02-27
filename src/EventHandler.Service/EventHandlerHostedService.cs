using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventHandler.Service
{
    public class EventHandlerHostedService : BackgroundService
    {
        private const int TaskTimeoutFromMiliseconds = 3000;

        private Task? _ccsTask;
        private readonly ILogger _logger;
        private readonly ISocketListenerFactory _socketListenerFactory;
        private readonly IConfigurationFactory _configurationFactory;
        private readonly IControlCenterServiceOperationsHandler _ccsOperationsHandler;

        public EventHandlerHostedService(
            ILogger<EventHandlerHostedService> logger,
            ISocketListenerFactory socketListenerFactory,
            IConfigurationFactory configurationFactory,
            IControlCenterServiceOperationsHandler ccsOperationsHandler)
        {
            _logger = logger;
            _socketListenerFactory = socketListenerFactory;
            _configurationFactory = configurationFactory;
            _ccsOperationsHandler = ccsOperationsHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Executing main task");

                var config = await _configurationFactory.PrepareConfigAsync();
                var sockets = config.EzEventHandler?.EventHandlerRawListOfSockets;
                var ccsSettings = config.EzEventHandler?.ControlCenterService;

                if (sockets is null ||
                    ccsSettings is null)
                {
                    _logger.LogError("Configuration object is empty");
                    await StopAsync(cancellationToken);
                    return;
                }

                _ccsTask = Task.Factory.StartNew(async () =>
                            await _ccsOperationsHandler.StartCCSConnectionAsync(ccsSettings, cancellationToken),
                                cancellationToken,
                                TaskCreationOptions.LongRunning,
                                TaskScheduler.Default).Unwrap();

                var openPortHandler = new OpenPortHandler(_logger, _socketListenerFactory, sockets);

                await openPortHandler.InvokeAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred during executing service");
            }
            finally
            {
                _ccsTask?.Wait(TaskTimeoutFromMiliseconds);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop requested, closing {SoftwareName} v.{SoftwareVersion}",
                AssemblyInformation.SoftwareName, AssemblyInformation.SoftwareVersion);

            _ccsOperationsHandler.StopCCSConnection();

            return base.StopAsync(cancellationToken);
        }
    }
}