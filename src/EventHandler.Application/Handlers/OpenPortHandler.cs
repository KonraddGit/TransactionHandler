using EventHandler.Application.Configuration;
using EventHandler.Application.Contracts.Infrastructure.Factory;
using EventHandler.Application.Contracts.Infrastructure.ProtocolListeners;
using EventHandler.Domain.Models.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventHandler.Application.Handlers
{
    public class OpenPortHandler
    {
        private readonly ILogger _logger;
        private readonly ISocketListenerFactory _socketListenerFactory;
        private readonly EventHandlerRawListOfSockets _listOfSockets;

        public OpenPortHandler(
            ILogger logger,
            ISocketListenerFactory socketListenerFactory,
            EventHandlerRawListOfSockets listOfSockets)
        {
            _logger = logger;
            _listOfSockets = listOfSockets;
            _socketListenerFactory = socketListenerFactory;
        }

        public async Task InvokeAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_listOfSockets.Sockets.Any() == false)
                {
                    _logger.LogError("Failed to open port, configuration is empty");
                    return;
                }

                var firewall = new FirewallConfiguration(_logger, _listOfSockets);
                firewall.OpenFirewallPorts();

                var listeningTasks = GetSocketListeners()
                     .Select(socketListener => socketListener.ListenAsync(cancellationToken));

                await Task.WhenAll(listeningTasks);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred during initialization of port listeners");
            }
        }

        private IEnumerable<ISocketListener> GetSocketListeners()
        {
            var sockets = new Dictionary<int, ISocketListener>();

            foreach (var socket in _listOfSockets.Sockets)
            {
                if (socket.SocketConfigurationEnabled == false)
                {
                    _logger.LogInformation("Socket {Port} is disabled", socket.Port);
                    continue;
                }

                if (sockets.ContainsKey(socket.Port))
                {
                    _logger.LogError("Configuration has duplicated sockets with port: {Port}", socket.Port);
                }
                else
                {
                    var socketListener = _socketListenerFactory.GetSocketListener(socket);

                    sockets.Add(socket.Port, socketListener);
                }
            }

            return sockets.Values;
        }
    }
}
