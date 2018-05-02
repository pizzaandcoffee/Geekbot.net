using System.Threading.Tasks;
using Discord;

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
            var logMessage = $"[{message.Source}] {message.Message}";
            switch (message.Severity)
            {
                case LogSeverity.Verbose:
                    _logger.Trace(message.Source, message.Message);
                    break;
                case LogSeverity.Debug:
                    _logger.Debug(message.Source, message.Message);
                    break;
                case LogSeverity.Info:
                    _logger.Information(message.Source, message.Message);
                    break;
                case LogSeverity.Critical:
                case LogSeverity.Error:
                case LogSeverity.Warning:
                    if (logMessage.Contains("VOICE_STATE_UPDATE")) break;
                    _logger.Error(message.Source, message.Message, message.Exception);
                    break;
                default:
                    _logger.Information(message.Source, $"{logMessage} --- {message.Severity}");
                    break;
            }
            return Task.CompletedTask;
        }
    }
}