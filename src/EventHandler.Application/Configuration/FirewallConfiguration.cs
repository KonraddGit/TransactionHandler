using EventHandler.Application.Extensions;
using EventHandler.Domain.Models.Configuration;
using EventHandler.Domain.Models.Configuration.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WindowsFirewallHelper;

namespace EventHandler.Application.Configuration
{
    public class FirewallConfiguration
    {
        private readonly ILogger _logger;
        private readonly EventHandlerRawListOfSockets _rawSockets;

        public FirewallConfiguration(
            ILogger logger,
            EventHandlerRawListOfSockets rawSockets)
        {
            _logger = logger;
            _rawSockets = rawSockets;
        }

        public void OpenFirewallPorts()
        {
            try
            {
                var ports = GetSocketPortsWithProtocolType();
                var portsToOpen = GetPortsToOpen(ports);

                if (portsToOpen.Any())
                {
                    OpenPorts(portsToOpen);
                }
                else
                {
                    _logger.LogInformation("No port is required to be open");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Open firewall ports failed");
            }
        }

        private void OpenPorts(IEnumerable<(ushort Port, ProtocolType ProtocolType)> portsToOpen)
        {
            foreach (var socket in portsToOpen)
            {
                var firewallProtocol = GetFirewallProtocol(socket.ProtocolType);

                var rule = FirewallManager.Instance.CreatePortRule(
                            $"EventHandler",
                            FirewallAction.Allow,
                            socket.Port);

                rule.Protocol = firewallProtocol;

                FirewallManager.Instance.Rules.Add(rule);

                _logger.LogInformation("Port: {Port} added to firewall", socket.Port);
            }
        }

        private IEnumerable<(ushort Port, ProtocolType ProtocolType)> GetPortsToOpen(IEnumerable<(ushort Port, ProtocolType ProtocolType)> ports)
        {
            var portsToOpen = new List<(ushort Port, ProtocolType ProtocolType)>();
            var rules = FirewallManager.Instance.Rules.ToList();

            foreach (var port in ports)
            {
                if (!rules.Any(x => x.LocalPorts.Contains(port.Port)))
                {
                    portsToOpen.Add(port);
                }
                else
                {
                    _logger.LogInformation("Port: {Port} is already opened", port);
                }
            }

            return portsToOpen;
        }

        private IEnumerable<(ushort Port, ProtocolType ProtocolType)> GetSocketPortsWithProtocolType() =>
            _rawSockets.Sockets
            .Where(socket => socket.EnablePortOnFirewall &&
                             socket.Port.IsSafeToConvertIntToUShort() &&
                             socket.SocketConfigurationEnabled)
            .Select(socket =>
                (
                    (ushort)socket.Port,
                    socket.ProtocolType
                ));

        private FirewallProtocol GetFirewallProtocol(ProtocolType protocolType)
            => protocolType switch
            {
                ProtocolType.Tcp => FirewallProtocol.TCP,
                ProtocolType.Udp => FirewallProtocol.UDP,
                _ => FirewallProtocol.Any
            };
    }
}