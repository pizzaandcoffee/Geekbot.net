using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Geekbot.Core.Logger
{
    public class GeekbotLogger : IGeekbotLogger
    {
        private readonly bool _logAsJson;
        private readonly NLog.Logger _logger;
        private readonly JsonSerializerOptions _serializerSettings;

        public GeekbotLogger(RunParameters runParameters)
        {
            _logAsJson = !string.IsNullOrEmpty(runParameters.SumologicEndpoint) || runParameters.LogJson;
            _logger = LoggerFactory.CreateNLog(runParameters);
            _serializerSettings = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            };
            Information(LogSource.Geekbot, "Using GeekbotLogger");
        }

        public void Trace(LogSource source, string message, object extra = null)
            => _logger.Trace(CreateLogString("Trace", source, message, null, extra));

        public void Debug(LogSource source, string message, object extra = null)
            => _logger.Debug(CreateLogString("Debug", source, message, null, extra));

        public void Information(LogSource source, string message, object extra = null)
            => _logger.Info(CreateLogString("Information", source, message, null, extra));

        public void Warning(LogSource source, string message, Exception stackTrace = null, object extra = null)
            => _logger.Warn(CreateLogString("Warning", source, message, stackTrace, extra));

        public void Error(LogSource source, string message, Exception stackTrace, object extra = null)
            => _logger.Error(stackTrace, CreateLogString("Error", source, message, stackTrace, extra));

        public NLog.Logger GetNLogger() => _logger;

        public bool LogAsJson() => _logAsJson;

        private string CreateLogString(string type, LogSource source, string message, Exception exception = null, object extra = null)
        {
            if (_logAsJson)
            {
                var logObject = new GeekbotLoggerObject
                {
                    Timestamp = DateTime.Now,
                    Type = type,
                    Source = source,
                    Message = message,
                    StackTrace = exception != null ? new ExceptionDto(exception) : null,
                    Extra = extra
                };
                return JsonSerializer.Serialize(logObject, _serializerSettings);
            }

            if (source != LogSource.Message) return $"[{source}] - {message}";

            var m = (MessageDto) extra;
            return $"[{source}] - [{m?.Guild.Name} - {m?.Channel.Name}] {m?.User.Name}: {m?.Message.Content}";
        }
    }
}