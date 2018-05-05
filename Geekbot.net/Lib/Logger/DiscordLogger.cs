using System;
using System.Threading.Tasks;
using Discord;
using Geekbot.net.Commands.Randomness.Cat;

namespace Geekbot.net.Lib.Logger
{
    public class DiscordLogger : IDiscordLogger
    {
        private readonly GeekbotLogger _logger;

        public DiscordLogger(GeekbotLogger logger)
        {
            _logger = logger;
        }
        
        public Task Log(LogMessage message)
        {
            LogSource source;
            try
            {
                source = Enum.Parse<LogSource>(message.Source);
            }
            catch
            {
                source = LogSource.Discord;
                _logger.Warning(LogSource.Geekbot, $"Could not parse {message.Source} to a LogSource Enum");
            }
            
            var logMessage = $"[{message.Source}] {message.Message}";
            switch (message.Severity)
            {
                case LogSeverity.Verbose:
                    _logger.Trace(source, message.Message);
                    break;
                case LogSeverity.Debug:
                    _logger.Debug(source, message.Message);
                    break;
                case LogSeverity.Info:
                    _logger.Information(source, message.Message);
                    break;
                case LogSeverity.Critical:
                case LogSeverity.Error:
                case LogSeverity.Warning:
                    if (logMessage.Contains("VOICE_STATE_UPDATE")) break;
                    _logger.Error(source, message.Message, message.Exception);
                    break;
                default:
                    _logger.Information(source, $"{logMessage} --- {message.Severity}");
                    break;
            }
            return Task.CompletedTask;
        }
    }
}