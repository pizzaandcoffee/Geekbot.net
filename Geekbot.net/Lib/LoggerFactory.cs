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