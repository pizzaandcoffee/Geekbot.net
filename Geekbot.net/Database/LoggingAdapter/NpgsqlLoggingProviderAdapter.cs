using Geekbot.net.Lib.Logger;
using Npgsql.Logging;

namespace Geekbot.net.Database.LoggingAdapter
{
    public class NpgsqlLoggingProviderAdapter : INpgsqlLoggingProvider
    {
        private readonly GeekbotLogger _geekbotLogger;

        public NpgsqlLoggingProviderAdapter(GeekbotLogger geekbotLogger)
        {
            _geekbotLogger = geekbotLogger;
        }
        
        public NpgsqlLogger CreateLogger(string name)
        {
            return new NpgsqlLoggingAdapter(name, _geekbotLogger);
        }
    }
}