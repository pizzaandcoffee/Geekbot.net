using System;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Logger;
using Npgsql.Logging;
using LogLevel = NLog.LogLevel;

namespace Geekbot.net.Database.LoggingAdapter
{
    public class NpgsqlLoggingAdapter : NpgsqlLogger
    {
        private readonly string _name;
        private readonly IGeekbotLogger _geekbotLogger;
        private readonly RunParameters _runParameters;

        public NpgsqlLoggingAdapter(string name, IGeekbotLogger geekbotLogger, RunParameters runParameters)
        {
            _name = name.Substring(7);
            _geekbotLogger = geekbotLogger;
            _runParameters = runParameters;
            geekbotLogger.Trace(LogSource.Database, $"Loaded Npgsql logging adapter: {name}");
        }
        
        public override bool IsEnabled(NpgsqlLogLevel level)
        {
            return (_runParameters.DbLogging && _geekbotLogger.GetNLogger().IsEnabled(ToGeekbotLogLevel(level)));
        }

        public override void Log(NpgsqlLogLevel level, int connectorId, string msg, Exception exception = null)
        {
            var nameAndMessage = $"{_name}: {msg}";
            switch (level)
            {
                case NpgsqlLogLevel.Trace:
                    _geekbotLogger.Trace(LogSource.Database, nameAndMessage);
                    break;
                case NpgsqlLogLevel.Debug:
                    _geekbotLogger.Debug(LogSource.Database, nameAndMessage);
                    break;
                case NpgsqlLogLevel.Info:
                    _geekbotLogger.Information(LogSource.Database, nameAndMessage);
                    break;
                case NpgsqlLogLevel.Warn:
                    _geekbotLogger.Warning(LogSource.Database, nameAndMessage, exception);
                    break;
                case NpgsqlLogLevel.Error:
                case NpgsqlLogLevel.Fatal:
                    _geekbotLogger.Error(LogSource.Database, nameAndMessage, exception);
                    break;
                default:
                    _geekbotLogger.Information(LogSource.Database, nameAndMessage);
                    break;
            }
        }

        private static LogLevel ToGeekbotLogLevel(NpgsqlLogLevel level)
        {
            return level switch
            {
                NpgsqlLogLevel.Trace => LogLevel.Trace,
                NpgsqlLogLevel.Debug => LogLevel.Debug,
                NpgsqlLogLevel.Info => LogLevel.Info,
                NpgsqlLogLevel.Warn => LogLevel.Warn,
                NpgsqlLogLevel.Error => LogLevel.Error,
                NpgsqlLogLevel.Fatal => LogLevel.Fatal,
                _ => throw new ArgumentOutOfRangeException(nameof(level))
            };
        }
    }
}