using System;
using Newtonsoft.Json;
using Serilog;

namespace Geekbot.net.Lib
{
    public class GeekbotLogger : IGeekbotLogger
    {
        private readonly bool _sumologicActive;
        private readonly ILogger _serilog;
        private readonly JsonSerializerSettings _serializerSettings;

        public GeekbotLogger(bool sumologicActive)
        {
            _sumologicActive = sumologicActive;
            _serilog = LoggerFactory.CreateLogger(sumologicActive);
            _serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                NullValueHandling = NullValueHandling.Include
            };
            Information("Geekbot", "Using GeekbotLogger");
        }
        
        public void Debug(string source, string message, object extra = null)
        {
            _serilog.Debug(CreateLogString("Debug", source, message, null, extra));
        }
        
        public void Information(string source, string message, object extra = null)
        {
            _serilog.Information(CreateLogString("Information", source, message, null, extra));
        }
        
        public void Warning(string source, string message, Exception stackTrace = null, object extra = null)
        {
            _serilog.Warning(CreateLogString("Warning", source, message, stackTrace, extra));
        }
        
        public void Error(string source, string message, Exception stackTrace, object extra = null)
        {
            _serilog.Error(CreateLogString("Error", source, message, stackTrace, extra));
        }

        private void HandleLogObject(string type, string source, string message, Exception stackTrace = null, object extra = null)
        {
            var logJson = CreateLogString(type, source, message, stackTrace, extra);
            // fuck serilog
            _serilog.Information(logJson, stackTrace + "}");
        }

        private string CreateLogString(string type, string source, string message, Exception stackTrace = null, object extra = null)
        {
            if (_sumologicActive)
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
                return JsonConvert.SerializeObject(logObject, Formatting.None, _serializerSettings);
            }
            
            if (source != "Message") return $"[{source}] - {message}";
            
            var m = (MessageDto) extra; 
            return $"[{source}] - [{m.Guild.Name} - {m.Channel.Name}] {m.User.Name}: {m.Message.Content}";
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