using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.Database;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.GuildSettingsManager;
using Geekbot.Core.KvInMemoryStore;
using Geekbot.Core.RandomNumberGenerator;
using Sentry;

namespace Geekbot.Bot.Commands.Games.Roll
{
    public class Roll : GeekbotCommandBase
    {
        private readonly IKvInMemoryStore _kvInMemoryStore;
        private readonly DatabaseContext _database;
        private readonly IRandomNumberGenerator _randomNumberGenerator;

        public Roll(IKvInMemoryStore kvInMemoryStore, IErrorHandler errorHandler, DatabaseContext database, IRandomNumberGenerator randomNumberGenerator, IGuildSettingsManager guildSettingsManager)
            : base(errorHandler, guildSettingsManager)
        {
            _kvInMemoryStore = kvInMemoryStore;
            _database = database;
            _randomNumberGenerator = randomNumberGenerator;
        }

        [Command("roll", RunMode = RunMode.Async)]
        [Summary("Guess which number the bot will roll (1-100")]
        public async Task RollCommand([Remainder] [Summary("guess")] string stuff = null)
        {
            try
            {
                var res = await new Geekbot.Commands.Roll.Roll(_kvInMemoryStore, _database, _randomNumberGenerator)
                    .RunFromGateway(
                        Context.Guild.Id,
                        Context.User.Id,
                        Context.User.Username, 
                        stuff ?? "0"
                    );
                await ReplyAsync(res);
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
                Transaction.Status = SpanStatus.InternalError;
            }
        }
    }
}