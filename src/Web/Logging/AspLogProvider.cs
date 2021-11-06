using System.Collections.Concurrent;
using Geekbot.Core.Logger;

namespace Geekbot.Web.Logging;

public class AspLogProvider : ILoggerProvider
{
    private readonly IGeekbotLogger _geekbotLogger;

    private readonly ConcurrentDictionary<string, AspLogger> _loggers = new ConcurrentDictionary<string, AspLogger>();

    public AspLogProvider(IGeekbotLogger geekbotLogger)
    {
        _geekbotLogger = geekbotLogger;
    }

    public void Dispose()
    {
        _loggers.Clear();
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new AspLogger(categoryName, _geekbotLogger));
    }
}