using Geekbot.Core.Logger;

namespace Geekbot.Web.Logging;

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
            case LogLevel.None:
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