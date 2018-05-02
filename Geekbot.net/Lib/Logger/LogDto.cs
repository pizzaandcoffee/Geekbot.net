using System;

namespace Geekbot.net.Lib.Logger
{
    public class GeekbotLoggerObject
    {
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public Exception StackTrace { get; set; }
        public object Extra { get; set; }
    }
}