using EventHandler.Application.Contracts.Infrastructure.Persistence;
using EventHandler.Application.Detectors;
using EventHandler.Domain.Models.Configuration;
using EventHandler.Domain.Models.Configuration.Enums;
using EventHandler.Domain.Models.Persistence;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EventHandler.Application.Handlers
{
    public class MessageHandler
    {
        private readonly ILogger _logger;
        private static readonly NLog.ILogger _receivedDataLogger = NLog.LogManager.GetLogger("ReceivedDataFile");
        private readonly IMessageRepository _messageRepository;
        private readonly Dictionary<string, string> _buffers;
        private readonly EventHandlerRawSocket _rawSocket;

        public MessageHandler(
            ILogger logger,
            IMessageRepository messageRepository,
            EventHandlerRawSocket rawSocket)
        {
            _logger = logger;
            _messageRepository = messageRepository;
            _rawSocket = rawSocket;
            _buffers = new Dictionary<string, string>();
        }

        public async Task<SaveMessageType> HandleAsync(string message, IPEndPoint endPoint)
        {
            try
            {
                _receivedDataLogger.Trace("Port: {Port} | Source: {IPPort} | {message}", _rawSocket.Port, endPoint, message);

                var clientRecognition = GetClientRecognition(endPoint);

                if (await HandleImmediatelyAsync(_rawSocket, message, clientRecognition))
                {
                    return SaveMessageType.HandleImmediately;
                }

                if (!_buffers.ContainsKey(clientRecognition))
                {
                    _buffers.Add(clientRecognition, string.Empty);
                }

                var result = await HandleEndOfFileAsync(message, clientRecognition);

                await DetectBufferOverflowAsync(clientRecognition, _buffers[clientRecognition]);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling message");
                return SaveMessageType.None;
            }
        }

        private async Task<bool> HandleImmediatelyAsync(EventHandlerRawSocket rawSocket, string message, string clientRecognition)
        {
            if (rawSocket.HandleImmediately)
            {
                await _messageRepository.SaveMessageAsync(rawSocket, message, clientRecognition, SaveMessageType.HandleImmediately);
                return true;
            }

            return false;
        }

        private async Task DetectBufferOverflowAsync(string clientRecognition, string newBuffer)
        {
            if (BufferOverflowDetector.Detect(newBuffer, _rawSocket.BufferLength))
            {
                await _messageRepository.SaveMessageAsync(_rawSocket, newBuffer, clientRecognition, SaveMessageType.BufferOverflow);
                ClearBuffer(clientRecognition);
            }
        }

        private async Task<SaveMessageType> HandleEndOfFileAsync(string message, string clientRecognition)
        {
            var eofHandlingResult = new EndOfFileCharacterHandler(
                                            _logger,
                                             _buffers[clientRecognition],
                                            _rawSocket.EOFCharacters,
                                            message)
                                            .ReturnEofDetectionResult();

            var persistingTasks = eofHandlingResult.CompletedMessages.Select(batch
               => _messageRepository.SaveMessageAsync(_rawSocket, batch, clientRecognition, SaveMessageType.EndOfFileCharacter));

            await Task.WhenAll(persistingTasks);

            UpdateBuffer(clientRecognition, eofHandlingResult.BufferNewValue);

            if (eofHandlingResult.CompletedMessages.Any())
            {
                return SaveMessageType.EndOfFileCharacter;
            }

            return SaveMessageType.None;
        }

        private void UpdateBuffer(string endPoint, string? message)
            => _buffers[endPoint] = message ?? string.Empty;

        private void ClearBuffer(string endPoint)
            => _buffers[endPoint] = string.Empty;

        private string GetClientRecognition(IPEndPoint endPoint)
            => _rawSocket.ClientRecognitionType switch
            {
                ClientRecognitionType.IPAddress => endPoint.Address.ToString(),
                ClientRecognitionType.Full => endPoint.ToString(),
                _ => throw new NotSupportedException($"Client recognition of {_rawSocket.ClientRecognitionType} is not supported")
            };
    }
}