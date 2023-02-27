using Dapper;
using EventHandler.Application.Contracts.Infrastructure.Persistence;
using EventHandler.Domain.Models.Configuration;
using EventHandler.Domain.Models.Persistence;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EventHandler.Infrastructure.Persistence
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ILogger _logger;

        public MessageRepository(
            ILogger<MessageRepository> logger)
        {
            _logger = logger;
        }

        public async Task SaveMessageAsync(EventHandlerRawSocket rawSocket, string message, string endPoint, SaveMessageType saveMessageType)
        {
            await using var conn = new SqlConnection(rawSocket.DbConnectionString);
            await conn.OpenAsync();

            var parameters = new DynamicParameters();
            parameters.Add("@ipAddress", endPoint);
            parameters.Add("@data", message);
            parameters.Add("@customName", rawSocket.CustomName);
            parameters.Add("@listeningPort", rawSocket.Port);

            await conn.ExecuteAsync(rawSocket.StoredProcedureName, parameters, commandType: CommandType.StoredProcedure);

            Log(message, saveMessageType);
        }

        private void Log(string message, SaveMessageType saveMessageType)
        {
            var messageLogType = SaveMessageLogType(saveMessageType);

            _logger.LogInformation("Data inserted into database | Length={Length} | MessageLogType={messageLogType}",
                message.Length, messageLogType);
            _logger.LogTrace("Inserted data body: {message}", message);
        }

        private string SaveMessageLogType(SaveMessageType saveMessageType)
            => saveMessageType switch
            {
                SaveMessageType.BufferOverflow => "Overflown buffer detected",
                SaveMessageType.HandleImmediately => "Handle immediately flag detected",
                SaveMessageType.EndOfFileCharacter => "End of file character detected",
                _ => throw new System.NotImplementedException()
            };
    }
}
