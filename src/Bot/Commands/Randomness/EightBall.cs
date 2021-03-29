using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.GuildSettingsManager;

namespace Geekbot.Bot.Commands.Randomness
{
    public class EightBall : GeekbotCommandBase
    {
        public EightBall(IErrorHandler errorHandler, IGuildSettingsManager guildSettingsManager) : base(errorHandler, guildSettingsManager)
        {
        }

        [Command("8ball", RunMode = RunMode.Async)]
        [Summary("Ask 8Ball a Question.")]
        public async Task Ball([Remainder] [Summary("question")] string echo)
        {
            try
            {
                var enumerator = Localization.EightBall.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true).GetEnumerator();
                var replies = new List<string>();
                while (enumerator.MoveNext())
                {
                    replies.Add(enumerator.Value?.ToString());
                }
                
                var answer = new Random().Next(replies.Count);
                await ReplyAsync(replies[answer]);
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
    }
}