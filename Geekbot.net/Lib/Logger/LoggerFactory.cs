using System;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;
using SumoLogic.Logging.NLog;

namespace Geekbot.net.Lib.Logger
{
    public class LoggerFactory
    {
        public static NLog.Logger CreateNLog(RunParameters runParameters, bool sumologicActive)
        {
            var config = new LoggingConfiguration();

            if (sumologicActive)
            {
                Console.WriteLine("Logging Geekbot Logs to Sumologic");
                config.LoggingRules.Add(
                    new LoggingRule("*", LogLevel.Debug, LogLevel.Fatal,
                        new SumoLogicTarget()
                        {
                            Url = Environment.GetEnvironmentVariable("GEEKBOT_SUMO"),
                            SourceName = "GeekbotLogger",
                            Layout = "${message}",
                            UseConsoleLog = false,
                            OptimizeBufferReuse = true,
                            Name = "Geekbot",
                            AppendException = false
                        })
                    );
            }
            else
            {
                var minLevel = runParameters.Verbose ? LogLevel.Trace : LogLevel.Info;
                config.LoggingRules.Add(
                    new LoggingRule("*", minLevel, LogLevel.Fatal,
                        new ColoredConsoleTarget
                        {
                            Name = "Console",
                            Encoding = Encoding.UTF8,
                            Layout = "[${longdate} ${level:format=FirstCharacter}] ${message} ${exception:format=toString}"
                        })
                    );
                
                config.LoggingRules.Add(
                    new LoggingRule("*", minLevel, LogLevel.Fatal,
                        new FileTarget
                        {
                            Name = "File",
                            Layout = "[${longdate} ${level}] ${message}",
                            Encoding = Encoding.UTF8,
                            LineEnding = LineEndingMode.Default,
                            MaxArchiveFiles = 30,
                            ArchiveNumbering = ArchiveNumberingMode.Date,
                            ArchiveEvery = FileArchivePeriod.Day,
                            ArchiveFileName = "./Logs/Archive/{#####}.log",
                            FileName = "./Logs/Geekbot.log"
                        })
                    );
            }
            
            var loggerConfig = new LogFactory { Configuration = config };
            return loggerConfig.GetCurrentClassLogger();
        }
    }
}