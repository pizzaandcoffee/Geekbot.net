using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Database;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.Levels;

namespace Geekbot.net.Commands.User
{
    public class GuildInfo : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly DatabaseContext _database;
        private readonly ILevelCalc _levelCalc;

        public GuildInfo(DatabaseContext database, ILevelCalc levelCalc, IErrorHandler errorHandler)
        {
            _database = database;
            _levelCalc = levelCalc;
            _errorHandler = errorHandler;
        }

        [Command("serverstats", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Statistics)]
        [Summary("Show some info about the bot.")]
        public async Task GetInfo()
        {
            try
            {
                var eb = new EmbedBuilder();
                eb.WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(Context.Guild.IconUrl)
                    .WithName(Context.Guild.Name));
                eb.WithColor(new Color(110, 204, 147));

                var created = Context.Guild.CreatedAt;
                var age = Math.Floor((DateTime.Now - created).TotalDays);

                var messages = _database.Messages
                    .Where(e => e.GuildId == Context.Guild.Id.AsLong())
                    .Sum(e => e.MessageCount);
                var level = _levelCalc.GetLevel((int) messages);

                eb.AddField("Server Age", $"{created.Day}/{created.Month}/{created.Year} ({age} days)");
                eb.AddInlineField("Level", level)
                    .AddInlineField("Messages", messages);

                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}