using Discord.Commands;
using Geekbot.Bot.CommandPreconditions;
using Geekbot.Core;
using Geekbot.Core.Database;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.GuildSettingsManager;
using Geekbot.Core.Highscores;

namespace Geekbot.Bot.Commands.User
{
    public class Rank : GeekbotCommandBase
    {
        private readonly IHighscoreManager _highscoreManager;
        private readonly DatabaseContext _database;

        public Rank(DatabaseContext database, IErrorHandler errorHandler, IHighscoreManager highscoreManager, IGuildSettingsManager guildSettingsManager)
            : base(errorHandler, guildSettingsManager)
        {
            _database = database;
            _highscoreManager = highscoreManager;
        }

        [Command("rank", RunMode = RunMode.Async)]
        [Summary("Get the highscore for various stats like message count, karma, correctly guessed roles, etc...")]
        [DisableInDirectMessage]
        public async Task RankCmd(
            [Summary("type")] string typeUnformated = "messages",
            [Summary("amount")] int amount = 10,
            [Summary("season")] string season = null)
        {
            try
            {
                var res = new Geekbot.Commands.Rank(_database, _highscoreManager)
                    .Run(typeUnformated, amount, season, Context.Guild.Id, Context.Guild.Name);
                await ReplyAsync(res);
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
    }
}