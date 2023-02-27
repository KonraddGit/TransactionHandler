using EventHandler.Application.Contracts.Application;
using EventHandler.Application.Contracts.Infrastructure.Persistence;
using EventHandler.Application.Extensions;
using EventHandler.Domain.Models.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace EventHandler.Application.Configuration
{
    public class DatabaseConfigurationProvider : IDatabaseConfigurationProvider
    {
        private readonly ILogger _logger;
        private readonly IConfigurationRepository _configurationRepository;

        private const string ConfigurationCellName = "ListOfSocketsToWatch";

        public DatabaseConfigurationProvider(
            ILogger<DatabaseConfigurationProvider> logger,
            IConfigurationRepository configurationRepository)
        {
            _logger = logger;
            _configurationRepository = configurationRepository;
        }

        public async Task<EventHandlerConfiguration?> ReadConfigurationAsync()
        {
            try
            {
                var dbConfig = await _configurationRepository.GetConfigurationFromDbAsync();

                var sockets = GetSocketsObject(dbConfig.Sections);
                var ccsSettings = GetCCSObject(dbConfig.Properties);

                return EventHandlerConfiguration.Create(sockets, ccsSettings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize database config");
                return null;
            }
        }

        private EventHandlerRawListOfSockets GetSocketsObject(IEnumerable<ConfigSection> sections)
        {
            var sockets = sections
               .FirstOrDefault(s => s.Name == ConfigurationCellName)?.Configuration;

            if (sockets is null)
            {
                _logger.LogError("Sockets configuration is empty, cannot deserialize to object");
                return new EventHandlerRawListOfSockets();
            }

            var xRoot = new XmlRootAttribute
            {
                ElementName = ConfigurationCellName,
                IsNullable = true
            };

            return XmlExtensions.Deserialize<EventHandlerRawListOfSockets>(sockets, xRoot);
        }

        private static ControlCenterServiceSettings GetCCSObject(IEnumerable<PathValue> properties)
        {
            var ccsSettings = new XElement("ControlCenterService");

            foreach (var property in properties)
            {
                ccsSettings.SetAttributeValue(property.KeyPath, property.Value);
            }

            var xRoot = new XmlRootAttribute
            {
                ElementName = "ControlCenterService",
                IsNullable = true
            };

            return XmlExtensions.Deserialize<ControlCenterServiceSettings>(ccsSettings.ToString(), xRoot);
        }
    }
}
