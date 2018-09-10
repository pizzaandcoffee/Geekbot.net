using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Bugsnag;
using Bugsnag.Payload;
using Discord.Commands;
using Discord.Net;
using Geekbot.net.Lib.GlobalSettings;
using Geekbot.net.Lib.Localization;
using Geekbot.net.Lib.Logger;
using SharpRaven;
using SharpRaven.Data;
using Exception = System.Exception;

namespace Geekbot.net.Lib.ErrorHandling
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly IGeekbotLogger _logger;
        private readonly ITranslationHandler _translation;
        private readonly IRavenClient _raven;
        private readonly bool _errorsInChat;
        private readonly IClient _bugsnag;

        public ErrorHandler(IGeekbotLogger logger, ITranslationHandler translation, IGlobalSettings globalSettings, bool errorsInChat)
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

            var bugsnagApiKey = globalSettings.GetKey("BugsnagApiKey");
            if (!string.IsNullOrEmpty(bugsnagApiKey))
            {
                _bugsnag = new Bugsnag.Client(new Bugsnag.Configuration
                {
                    ApiKey = bugsnagApiKey,
                    AppVersion = Constants.BotVersion()
                });
                _logger.Information(LogSource.Geekbot, "Command Errors will be logged to Bugsnag");
            }
            else
            {
                _bugsnag = null;
            }
        }

        public async Task HandleCommandException(Exception e, ICommandContext context, string errorMessage = "def")
        {
            try
            {
                var errorString = errorMessage == "def" ? await _translation.GetString(context.Guild?.Id ?? 0, "errorHandler", "SomethingWentWrong") : errorMessage;
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

        private void ReportExternal(Exception e, MessageDto errorObj)
        {
            if (_raven != null)
            {
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

            _bugsnag?.Notify(e, (report) =>
            {
                report.Event.Metadata.Add("Discord Location", new Dictionary<string, string>
                {
                    {"Guild Name", errorObj.Guild.Name},
                    {"Guild Id", errorObj.Guild.Id},
                    {"Channel Name", errorObj.Channel.Name},
                    {"Channel Id", errorObj.Channel.Id}
                });
                report.Event.Metadata.Add("Message Info", new Dictionary<string, string>
                {
                    {"Content", errorObj.Message.Content},
                    {"Id", errorObj.Message.Id},
                    {"Attachments", errorObj.Message.Attachments.ToString()},
                    {"ChannelMentions", errorObj.Message.ChannelMentions.ToString()},
                    {"UserMentions", errorObj.Message.UserMentions.ToString()},
                    {"RoleMentions", errorObj.Message.RoleMentions.ToString()},
                });
                report.Event.Severity = Severity.Error;
                report.Event.User = new User
                {
                    Id = errorObj.User.Id,
                    Name = errorObj.User.Name
                };
            });
        }
    }
}