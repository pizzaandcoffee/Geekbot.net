using System;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using Discord.Commands;
using Discord.Net;
using Nancy.Extensions;
using Serilog;
using SharpRaven;
using SharpRaven.Data;
using SharpRaven.Utilities;
using Utf8Json;

namespace Geekbot.net.Lib
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly IGeekbotLogger _logger;
        private readonly ITranslationHandler _translation;
        private readonly IRavenClient _raven;

        public ErrorHandler(IGeekbotLogger logger, ITranslationHandler translation)
        {
            _logger = logger;
            _translation = translation;
            
            var sentryDsn = Environment.GetEnvironmentVariable("SENTRY");
            if (!string.IsNullOrEmpty(sentryDsn))
            {
                _raven = new RavenClient(sentryDsn);
                _logger.Information("Geekbot", $"Command Errors will be logged to Sentry: {sentryDsn}");
            }
            else
            {
                _raven = null;
            }
        }

        public void HandleCommandException(Exception e, ICommandContext Context, string errorMessage = "def")
        {
            try
            {
                var errorString = errorMessage == "def" ? _translation.GetString(Context.Guild.Id, "errorHandler", "SomethingWentWrong") : errorMessage;
                var errorObj = SimpleConextConverter.ConvertContext(Context);
                _logger.Error("Geekbot", "An error ocured", e, errorObj);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Context.Channel.SendMessageAsync(errorString);
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
                _logger.Error("Geekbot", "Errorception", ex);
            }
        }

        public async void HandleHttpException(HttpException e, ICommandContext Context)
        {
            var errorStrings = _translation.GetDict(Context, "httpErrors");
            switch(e.HttpCode)
            {
                case HttpStatusCode.Forbidden:
                    await Context.Channel.SendMessageAsync(errorStrings["403"]);
                    break;
            }
        }

        
    }

    public interface IErrorHandler
    {
        void HandleCommandException(Exception e, ICommandContext Context, string errorMessage = "def");
        void HandleHttpException(HttpException e, ICommandContext Context);
    }
}