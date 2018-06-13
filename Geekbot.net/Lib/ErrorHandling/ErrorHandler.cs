using System;
using System.Net;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Net;
using Geekbot.net.Lib.Localization;
using Geekbot.net.Lib.Logger;
using SharpRaven;
using SharpRaven.Data;

namespace Geekbot.net.Lib.ErrorHandling
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly IGeekbotLogger _logger;
        private readonly ITranslationHandler _translation;
        private readonly IRavenClient _raven;
        private readonly bool _errorsInChat;

        public ErrorHandler(IGeekbotLogger logger, ITranslationHandler translation, bool errorsInChat)
        {
            _logger = logger;
            _translation = translation;
            _errorsInChat = errorsInChat;

            var sentryDsn = Environment.GetEnvironmentVariable("SENTRY");
            if (!string.IsNullOrEmpty(sentryDsn))
            {
                _raven = new RavenClient(sentryDsn) { Release = Constants.BotVersion() };
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
                var errorString = errorMessage == "def" ? await _translation.GetString(context.Guild.Id, "errorHandler", "SomethingWentWrong") : errorMessage;
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
                            context.Channel.SendMessageAsync($"{e.Message}\r\n```\r\n{resStackTrace.Substring(0, maxLen)}\r\n```");
                        }
                        else
                        {
                            context.Channel.SendMessageAsync(e.Message);
                        }
                    }
                    else
                    {
                        context.Channel.SendMessageAsync(errorString);
                    }
                    
                }
                
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
            catch (Exception ex)
            {
                context.Channel.SendMessageAsync("Something went really really wrong here");
                _logger.Error(LogSource.Geekbot, "Errorception", ex);
            }
        }

        public async Task HandleHttpException(HttpException e, ICommandContext context)
        {
            var errorStrings = await _translation.GetDict(context, "httpErrors");
            switch(e.HttpCode)
            {
                case HttpStatusCode.Forbidden:
                    await context.Channel.SendMessageAsync(errorStrings["403"]);
                    break;
            }
        }

        
    }
}