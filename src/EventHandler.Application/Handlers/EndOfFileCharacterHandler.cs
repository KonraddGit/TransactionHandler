using EventHandler.Domain.Models.Handlers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EventHandler.Application.Handlers
{
    public class EndOfFileCharacterHandler
    {
        private readonly ILogger _logger;
        private readonly string _buffer;
        private readonly string[] _eofCharacters;
        private readonly string _message;

        public EndOfFileCharacterHandler(
            ILogger logger,
            string buffer,
            string[] eofCharacters,
            string message
            )
        {
            _logger = logger;
            _buffer = buffer;
            _eofCharacters = eofCharacters;
            _message = message;
        }

        public EofDetectionResult ReturnEofDetectionResult()
        {
            try
            {
                var data = _buffer + _message;
                var pattern = PrepareRegexPattern();
                var batchesWithEof = new List<string>();
                var batches = Regex.Split(data, pattern);
                var tempBuff = string.Empty;

                foreach (var match in batches)
                {
                    if (Regex.Match(match, pattern).Success)
                    {
                        tempBuff += match;
                        batchesWithEof.Add(tempBuff);
                        tempBuff = string.Empty;
                    }
                    else
                    {
                        tempBuff += match;
                    }
                }

                return new EofDetectionResult
                {
                    CompletedMessages = batchesWithEof,
                    BufferNewValue = tempBuff
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling end of file character");
                throw;
            }
        }

        private string PrepareRegexPattern()
        {
            var escapedCharacters = _eofCharacters.Select(Regex.Escape).ToList();
            return $"({string.Join("|", escapedCharacters)})";
        }
    }
}
