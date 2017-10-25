using System;
using Serilog;
using System.Linq;

namespace Geekbot.net.Lib
{
    public class LoggerFactory
    {
        public static ILogger createLogger(string[] args)
        {
            var loggerCreation = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .WriteTo.RollingFile("Logs/geekbot-{Date}.txt", shared: true);
            var sentryDsn = Environment.GetEnvironmentVariable("SENTRY");
            if (!string.IsNullOrEmpty(sentryDsn))
            {
                loggerCreation.WriteTo.SentryIO(sentryDsn)
                    .Enrich.FromLogContext();
                Console.WriteLine($"Logging to Sentry Enabled: {sentryDsn}");
            }
            if (args.Contains("--verbose"))
            {
                loggerCreation.MinimumLevel.Verbose();
            }
            else
            {
                loggerCreation.MinimumLevel.Information();
            }
            return loggerCreation.CreateLogger();
        }
    }
}