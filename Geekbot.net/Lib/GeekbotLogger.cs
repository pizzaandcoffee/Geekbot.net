using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace Geekbot.net.Lib
{
    public class GeekbotLogger : IGeekbotLogger
    {
        private readonly ILogger _serilog;
        public GeekbotLogger()
        {
            _serilog = LoggerFactory.CreateLogger();
            Information("Geekbot", "Using GeekbotLogger");
        }
        
        public void Debug(string source, string message, object extra = null)
        {
            HandleLogObject("Debug", source, message, null, extra);
        }
        
        public void Information(string source, string message, object extra = null)
        {
            HandleLogObject("Information", source, message, null, extra);
        }
        
        public void Warning(string source, string message, Exception stackTrace = null, object extra = null)
        {
            HandleLogObject("Warning", source, message, stackTrace, extra);
        }
        
        public void Error(string source, string message, Exception stackTrace, object extra = null)
        {
            HandleLogObject("Error", source, message, stackTrace, extra);
        }

        private Task HandleLogObject(string type, string source, string message, Exception stackTrace = null, object extra = null)
        {
            var logJson = CreateLogObject(type, source, message, stackTrace, extra);
            // fuck serilog
            _serilog.Information(logJson + "}");
            return Task.CompletedTask;
        }

        private string CreateLogObject(string type, string source, string message, Exception stackTrace = null, object extra = null)
        {
            var logObject = new GeekbotLoggerObject
            {
                Timestamp = DateTime.Now,
                Type = type,
                Source = source,
                Message = message,
                StackTrace = stackTrace,
                Extra = extra
            };
            return JsonConvert.SerializeObject(logObject);
        }
    }

    public class GeekbotLoggerObject
    {
        public DateTime Timestamp { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
        public Exception StackTrace { get; set; }
        public object Extra { get; set; }
    }

    public interface IGeekbotLogger
    {
        void Debug(string source, string message, object extra = null);
        void Information(string source, string message, object extra = null);
        void Warning(string source, string message, Exception stackTrace = null, object extra = null);
        void Error(string source, string message, Exception stackTrace, object extra = null);
    }
}