using Geekbot.net.Lib;
using Geekbot.net.Lib.Logger;
using Npgsql.Logging;

namespace Geekbot.net.Database.LoggingAdapter
{
    public class NpgsqlLoggingProviderAdapter : INpgsqlLoggingProvider
    {
        private readonly GeekbotLogger _geekbotLogger;
        private readonly RunParameters _runParameters;

        public NpgsqlLoggingProviderAdapter(GeekbotLogger geekbotLogger, RunParameters runParameters)
        {
            _geekbotLogger = geekbotLogger;
            _runParameters = runParameters;
        }
        
        public NpgsqlLogger CreateLogger(string name)
        {
            return new NpgsqlLoggingAdapter(name, _geekbotLogger, _runParameters);
        }
    }
}