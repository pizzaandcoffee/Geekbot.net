using System;
using Newtonsoft.Json;

namespace Geekbot.Core.Logger
{
    public class GeekbotLogger : IGeekbotLogger
    {
        private readonly bool _logAsJson;
        private readonly NLog.Logger _logger;
        private readonly JsonSerializerSettings _serializerSettings;

        public GeekbotLogger(RunParameters runParameters)
        {
            _logAsJson = !string.IsNullOrEmpty(runParameters.SumologicEndpoint) || runParameters.LogJson;
            _logger = LoggerFactory.CreateNLog(runParameters);
            _serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                NullValueHandling = NullValueHandling.Include
            };
            Information(LogSource.Geekbot, "Using GeekbotLogger");
        }
        
        public void Trace(LogSource source, string message, object extra = null)
        {
            _logger.Trace(CreateLogString("Trace", source, message, null, extra));
        }
        
        public void Debug(LogSource source, string message, object extra = null)
        {
            if (_logAsJson) _logger.Info(CreateLogString("Debug", source, message, null, extra));
            else _logger.Debug(CreateLogString("Debug", source, message, null, extra));
        }
        
        public void Information(LogSource source, string message, object extra = null)
        {
            _logger.Info(CreateLogString("Information", source, message, null, extra));
        }
        
        public void Warning(LogSource source, string message, Exception stackTrace = null, object extra = null)
        {
            if (_logAsJson) _logger.Info(CreateLogString("Warning", source, message, stackTrace, extra));
            else _logger.Warn(CreateLogString("Warning", source, message, stackTrace, extra));
        }
        
        public void Error(LogSource source, string message, Exception stackTrace, object extra = null)
        {
            if (_logAsJson) _logger.Info(CreateLogString("Error", source, message, stackTrace, extra));
            else _logger.Error(stackTrace, CreateLogString("Error", source, message, stackTrace, extra));
        }

        public NLog.Logger GetNLogger()
        {
            return _logger;
        }

        public bool LogAsJson()
        {
            return _logAsJson;
        }

        private string CreateLogString(string type, LogSource source, string message, Exception stackTrace = null, object extra = null)
        {
            if (_logAsJson)
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
            
            if (source != LogSource.Message) return $"[{source}] - {message}";
            
            var m = (MessageDto) extra; 
            return $"[{source}] - [{m?.Guild.Name} - {m?.Channel.Name}] {m?.User.Name}: {m?.Message.Content}";
        }
    }
}