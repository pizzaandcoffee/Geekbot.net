using System;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.SumoLogic;

namespace Geekbot.net.Lib
{
    public class LoggerFactory
    {
        public static ILogger createLogger()
        {
            var loggerCreation = new LoggerConfiguration();
            var template = "{Message}{NewLine}";
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GEEKBOT_SUMO")))
            {
                Console.WriteLine("Logging Geekbot Logs to Sumologic");
                loggerCreation.WriteTo.SumoLogic(Environment.GetEnvironmentVariable("GEEKBOT_SUMO"), 
                    outputTemplate: template);
            }
            else
            {
                loggerCreation.WriteTo.LiterateConsole(outputTemplate: template);
                loggerCreation.WriteTo.RollingFile("Logs/geekbot-{Date}.txt", shared: true, outputTemplate: template);
            }
            return loggerCreation.CreateLogger();
        }
    }
}