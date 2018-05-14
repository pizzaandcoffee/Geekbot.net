using System;
using Geekbot.net.Lib.Logger;
using Microsoft.Extensions.Logging;

namespace Geekbot.net.WebApi.Logging
{
    public class AspLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly IGeekbotLogger _geekbotLogger;
        
        public AspLogger(string categoryName, IGeekbotLogger geekbotLogger)
        {
            geekbotLogger.Trace(LogSource.Api, $"Adding {categoryName}");
            _categoryName = categoryName;
            _geekbotLogger = geekbotLogger;
        }
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    _geekbotLogger.Trace(LogSource.Api, $"{eventId.Id} - {_categoryName} - {state}");
                    break;
                case LogLevel.Debug:
                    _geekbotLogger.Debug(LogSource.Api, $"{eventId.Id} - {_categoryName} - {state}");
                    break;
                case LogLevel.Information:
                    _geekbotLogger.Information(LogSource.Api, $"{eventId.Id} - {_categoryName} - {state}");
                    break;
                case LogLevel.Warning:
                    _geekbotLogger.Warning(LogSource.Api, $"{eventId.Id} - {_categoryName} - {state}", exception);
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    _geekbotLogger.Error(LogSource.Api, $"{eventId.Id} - {_categoryName} - {state}", exception);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return !_geekbotLogger.LogAsJson() && _geekbotLogger.GetNLogger().IsEnabled(ToGeekbotLogLevel(logLevel));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
        
        private static NLog.LogLevel ToGeekbotLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return NLog.LogLevel.Trace;
                case LogLevel.Debug:
                    return NLog.LogLevel.Debug;
                case LogLevel.Information:
                    return NLog.LogLevel.Info;
                case LogLevel.Warning:
                    return NLog.LogLevel.Warn;
                case LogLevel.Error:
                    return NLog.LogLevel.Error;
                case LogLevel.Critical:
                    return NLog.LogLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level));
            }
        }
    }
}