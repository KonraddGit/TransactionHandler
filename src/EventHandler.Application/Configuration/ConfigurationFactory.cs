using EventHandler.Application.Contracts.Application;
using EventHandler.Application.Exceptions;
using EventHandler.Domain.Models.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EventHandler.Application.Configuration
{
    public class ConfigurationFactory : IConfigurationFactory
    {
        private readonly ILogger _logger;
        private readonly IBackupManager _backupManager;
        private readonly IFileConfigurationProvider _fileConfigurationProvider;
        private readonly IDatabaseConfigurationProvider _databaseConfigurationProvider;
        private EventHandlerConfiguration? _eventHandlerConfiguration;
        private const string Pattern = "<[0-9]{2}>";

        public ConfigurationFactory(
            ILogger<ConfigurationFactory> logger,
            IBackupManager backupManager,
            IFileConfigurationProvider fileConfigurationProvider,
            IDatabaseConfigurationProvider databaseConfigurationProvider)
        {
            _logger = logger;
            _backupManager = backupManager;
            _fileConfigurationProvider = fileConfigurationProvider;
            _databaseConfigurationProvider = databaseConfigurationProvider;
        }

        public async Task<EventHandlerConfiguration> PrepareConfigAsync()
        {
            try
            {
                _logger.LogInformation("Loading configuration from database");

                var config = await _databaseConfigurationProvider.ReadConfigurationAsync();

                if (config != null)
                {
                    await _backupManager.CreateBackupAsync(config);
                    _eventHandlerConfiguration = config;

                    _logger.LogTrace("Succesfully loaded config from database");
                }
                else
                {
                    _logger.LogInformation("Loading configuration from backup");
                    _eventHandlerConfiguration = await _fileConfigurationProvider.ReadConfigurationAsync();
                }

                InitializeEofCharacters();
                return _eventHandlerConfiguration!;
            }
            catch (DeserializeXmlException e)
            {
                _logger.LogError(e, "Deserializing file failed due to an inner error");
                throw new DeserializeXmlException(e, "Deserializing file failed due to an inner error");
            }
        }

        private string[] ConvertEofCharacters(string eofCharacter)
        {
            if (string.IsNullOrWhiteSpace(eofCharacter))
            {
                throw new ConvertEofException();
            }

            var matchEvaluator = new MatchEvaluator(RawASCIIEvaluator);
            var result = Regex.Replace(eofCharacter, Pattern, matchEvaluator);

            return result.Split('|');
        }

        private static string RawASCIIEvaluator(Match match)
        {
            var result = match.Value
                .Replace("<", string.Empty)
                .Replace(">", string.Empty);

            return Convert.ToChar(Convert.ToInt32(result)).ToString();
        }

        private void InitializeEofCharacters()
        {
            var sockets = _eventHandlerConfiguration?.EzEventHandler?.EventHandlerRawListOfSockets;

            if (sockets is null)
            {
                return;
            }

            foreach (var socket in sockets.Sockets)
            {
                if (socket.EOFCharacter is null)
                {
                    return;
                }

                socket.EOFCharacters = ConvertEofCharacters(socket.EOFCharacter);
            }
        }
    }
}
