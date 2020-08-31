using System;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;
using SumoLogic.Logging.NLog;

namespace Geekbot.Core.Logger
{
    public class LoggerFactory
    {
        public static NLog.Logger CreateNLog(RunParameters runParameters)
        {
            var config = new LoggingConfiguration();
            var minLevel = runParameters.Verbose ? LogLevel.Trace : LogLevel.Info;

            if (!string.IsNullOrEmpty(runParameters.SumologicEndpoint))
            {
                Console.WriteLine("Logging Geekbot Logs to Sumologic");
                config.LoggingRules.Add(
                    new LoggingRule("*", minLevel, LogLevel.Fatal,
                        new SumoLogicTarget()
                        {
                            Url = runParameters.SumologicEndpoint,
                            SourceName = "GeekbotLogger",
                            Layout = "${message}",
                            UseConsoleLog = false,
                            OptimizeBufferReuse = true,
                            Name = "Geekbot"
                        })
                );
            }
            else if (runParameters.LogJson)
            {
                config.LoggingRules.Add(
                    new LoggingRule("*", minLevel, LogLevel.Fatal,
                        new ConsoleTarget
                        {
                            Name = "Console",
                            Encoding = Encoding.UTF8,
                            Layout = "${message}"
                        }
                    )
                );
            }
            else
            {
                config.LoggingRules.Add(
                    new LoggingRule("*", minLevel, LogLevel.Fatal,
                        new ColoredConsoleTarget
                        {
                            Name = "Console",
                            Encoding = Encoding.UTF8,
                            Layout = "[${longdate} ${level:format=FirstCharacter}] ${message} ${exception:format=toString}"
                        }
                    )
                );
            }

            var loggerConfig = new LogFactory {Configuration = config};
            return loggerConfig.GetCurrentClassLogger();
        }
    }
}