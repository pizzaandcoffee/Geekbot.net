using System;
using Microsoft.Extensions.Logging;

namespace Geekbot.Core.Logger.Adapters;

public class ILoggerAdapter : ILogger
{
    private readonly string _categoryName;
    private readonly LogSource _logSource;
    private readonly IGeekbotLogger _geekbotLogger;
    public ILoggerAdapter(string categoryName, LogSource logSource, IGeekbotLogger geekbotLogger)
    {
        _categoryName = categoryName;
        _logSource = logSource;
        _geekbotLogger = geekbotLogger;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        switch (logLevel)
        {
            case LogLevel.Trace:
                _geekbotLogger.Trace(_logSource, $"{eventId.Id} - {_categoryName} - {state}");
                break;
            case LogLevel.Debug:
                _geekbotLogger.Debug(_logSource, $"{eventId.Id} - {_categoryName} - {state}");
                break;
            case LogLevel.Information:
                _geekbotLogger.Information(_logSource, $"{eventId.Id} - {_categoryName} - {state}");
                break;
            case LogLevel.Warning:
                _geekbotLogger.Warning(_logSource, $"{eventId.Id} - {_categoryName} - {state}", exception);
                break;
            case LogLevel.Error:
            case LogLevel.Critical:
                _geekbotLogger.Error(_logSource, $"{eventId.Id} - {_categoryName} - {state}", exception);
                break;
            case LogLevel.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(logLevel));
        }
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _geekbotLogger.GetNLogger().IsEnabled(ToGeekbotLogLevel(logLevel));
        // return !_geekbotLogger.LogAsJson() && _geekbotLogger.GetNLogger().IsEnabled(ToGeekbotLogLevel(logLevel));
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    private static NLog.LogLevel ToGeekbotLogLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => NLog.LogLevel.Trace,
            LogLevel.Debug => NLog.LogLevel.Debug,
            LogLevel.Information => NLog.LogLevel.Info,
            LogLevel.Warning => NLog.LogLevel.Warn,
            LogLevel.Error => NLog.LogLevel.Error,
            LogLevel.Critical => NLog.LogLevel.Fatal,
            LogLevel.None => NLog.LogLevel.Off,
            _ => throw new ArgumentOutOfRangeException(nameof(level))
        };
    }
}