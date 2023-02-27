using System.Collections.Generic;

namespace EventHandler.Domain.Models.Handlers
{
    public class EofDetectionResult
    {
        public IEnumerable<string> CompletedMessages { get; set; } = new List<string>();
        public string? BufferNewValue { get; set; }
    }
}
