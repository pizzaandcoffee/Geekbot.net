using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Geekbot.Core.Logger.Adapters;

public class ILoggerProviderProvider : ILoggerProvider, ILoggerFactory
{
    private readonly IGeekbotLogger _geekbotLogger;
    private readonly LogSource _logSource;

    private readonly ConcurrentDictionary<string, ILoggerAdapter> _loggers = new();

    public ILoggerProviderProvider(IGeekbotLogger geekbotLogger, LogSource logSource)
    {
        _geekbotLogger = geekbotLogger;
        _logSource = logSource;
    }

    public void Dispose()
    {
        _loggers.Clear();
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new ILoggerAdapter(categoryName, _logSource, _geekbotLogger));
    }
    
    public void AddProvider(ILoggerProvider provider)
    {
        throw new System.NotImplementedException();
    }
}