using System;

namespace Geekbot.Core.Logger
{
    public class GeekbotLoggerObject
    {
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
        public LogSource Source { get; set; }
        public string Message { get; set; }
        public Exception StackTrace { get; set; }
        public object Extra { get; set; }
    }
}