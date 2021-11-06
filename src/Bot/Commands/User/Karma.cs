using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Commands.Karma;
using Geekbot.Core;
using Geekbot.Core.CommandPreconditions;
using Geekbot.Core.Database;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.GuildSettingsManager;

namespace Geekbot.Bot.Commands.User
{
    [DisableInDirectMessage]
    public class Karma : GeekbotCommandBase
    {
        private readonly DatabaseContext _database;

        public Karma(DatabaseContext database, IErrorHandler errorHandler, IGuildSettingsManager guildSettingsManager) : base(errorHandler, guildSettingsManager)
        {
            _database = database;
        }

        [Command("good", RunMode = RunMode.Async)]
        [Summary("Increase Someones Karma")]
        public async Task Good([Summary("@someone")] IUser user)
        {
            await ChangeKarma(user, KarmaChange.Up);
        }

        [Command("bad", RunMode = RunMode.Async)]
        [Summary("Decrease Someones Karma")]
        public async Task Bad([Summary("@someone")] IUser user)
        {
            await ChangeKarma(user, KarmaChange.Down);
        }

        [Command("neutral", RunMode = RunMode.Async)]
        [Summary("Do nothing to someones Karma")]
        public async Task Neutral([Summary("@someone")] IUser user)
        {
            await ChangeKarma(user, KarmaChange.Same);
        }

        private async Task ChangeKarma(IUser user, KarmaChange change)
        {
            try
            {
                var author = new Interactions.Resolved.User()
                {
                    Id = Context.User.Id.ToString(),
                    Username = Context.User.Username,
                    Discriminator = Context.User.Discriminator,
                    Avatar = Context.User.AvatarId,
                };
                var targetUser = new Interactions.Resolved.User()
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Discriminator = user.Discriminator,
                    Avatar = user.AvatarId,
                };
                
                var karma = new Geekbot.Commands.Karma.Karma(_database, Context.Guild.Id.AsLong());
                var res = await karma.ChangeKarma(author, targetUser, change);

                await ReplyAsync(string.Empty, false, res.ToDiscordNetEmbed().Build());
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
    }
}