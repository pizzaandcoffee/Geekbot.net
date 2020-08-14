using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Core.Logger;
using SharpRaven;
using SharpRaven.Data;
using Exception = System.Exception;

namespace Geekbot.Core.ErrorHandling
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly IGeekbotLogger _logger;
        private readonly Func<string> _getDefaultErrorText;
        private readonly IRavenClient _raven;
        private readonly bool _errorsInChat;

        public ErrorHandler(IGeekbotLogger logger, RunParameters runParameters, Func<string> getDefaultErrorText)
        {
            _logger = logger;
            _getDefaultErrorText = getDefaultErrorText;
            _errorsInChat = runParameters.ExposeErrors;

            var sentryDsn = runParameters.SentryEndpoint;
            if (!string.IsNullOrEmpty(sentryDsn))
            {
                _raven = new RavenClient(sentryDsn) { Release = Constants.BotVersion(), Environment = "Production" };
                _logger.Information(LogSource.Geekbot, $"Command Errors will be logged to Sentry: {sentryDsn}");
            }
            else
            {
                _raven = null;
            }
        }

        public async Task HandleCommandException(Exception e, ICommandContext context, string errorMessage = "def")
        {
            try
            {
                var errorString = errorMessage == "def"
                    ? _getDefaultErrorText()
                    : errorMessage;
                var errorObj = SimpleConextConverter.ConvertContext(context);
                if (e.Message.Contains("50007")) return;
                if (e.Message.Contains("50013")) return;
                _logger.Error(LogSource.Geekbot, "An error ocured", e, errorObj);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    if (_errorsInChat)
                    {
                        var resStackTrace = string.IsNullOrEmpty(e.InnerException?.ToString()) ? e.StackTrace : e.InnerException?.ToString();
                        if (!string.IsNullOrEmpty(resStackTrace))
                        {
                            var maxLen = Math.Min(resStackTrace.Length, 1850);
                            await context.Channel.SendMessageAsync($"{e.Message}\r\n```\r\n{resStackTrace.Substring(0, maxLen)}\r\n```");
                        }
                        else
                        {
                            await context.Channel.SendMessageAsync(e.Message);
                        }
                    }
                    else
                    {
                        await context.Channel.SendMessageAsync(errorString);
                    }
                    
                }

                ReportExternal(e, errorObj);
            }
            catch (Exception ex)
            {
                await context.Channel.SendMessageAsync("Something went really really wrong here");
                _logger.Error(LogSource.Geekbot, "Errorception", ex);
            }
        }

        private void ReportExternal(Exception e, MessageDto errorObj)
        {
            if (_raven == null) return;
            var sentryEvent = new SentryEvent(e)
            {
                Tags =
                {
                    ["discord_server"] = errorObj.Guild.Name,
                    ["discord_user"] = errorObj.User.Name
                },
                Message = errorObj.Message.Content,
                Extra = errorObj
            };
            _raven.Capture(sentryEvent);
        }
    }
}