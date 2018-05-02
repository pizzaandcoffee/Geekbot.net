using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.UserRepository;
using OverwatchAPI;
using OverwatchAPI.Config;

namespace Geekbot.net.Commands.Games
{
    [Group("ow")]
    public class Overwatch : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IUserRepository _userRepository;

        public Overwatch(IErrorHandler errorHandler, IUserRepository userRepository)
        {
            _errorHandler = errorHandler;
            _userRepository = userRepository;
        }

        [Command("profile", RunMode = RunMode.Async)]
        [Summary("Get someones overwatch profile. EU on PC only. Default battletag is your own (if set).")]
        [Remarks(CommandCategories.Games)]
        public async Task OwProfile()
        {
            try
            {
                var tag = _userRepository.GetUserSetting(Context.User.Id, "BattleTag");
                if (string.IsNullOrEmpty(tag))
                {
                    await ReplyAsync("You have no battle Tag saved, use `!battletag`");
                    return;
                }

                var profile = await CreateProfile(tag);
                if (profile == null)
                {
                    await ReplyAsync("That player doesn't seem to exist");
                    return;
                }

                await ReplyAsync("", false, profile.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("profile", RunMode = RunMode.Async)]
        [Summary("Get someones overwatch profile. EU on PC only. Default battletag is your own (if set).")]
        [Remarks(CommandCategories.Games)]
        public async Task OwProfile([Summary("BattleTag")] string tag)
        {
            try
            {
                if (!BattleTag.IsValidTag(tag))
                {
                    await ReplyAsync("That doesn't seem to be a valid battletag...");
                    return;
                }

                var profile = await CreateProfile(tag);
                if (profile == null)
                {
                    await ReplyAsync("That player doesn't seem to exist");
                    return;
                }

                await ReplyAsync("", false, profile.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("profile", RunMode = RunMode.Async)]
        [Summary("Get someones overwatch profile. EU on PC only.")]
        [Remarks(CommandCategories.Games)]
        public async Task OwProfile([Summary("@someone")] IUser user)
        {
            try
            {
                var tag = _userRepository.GetUserSetting(user.Id, "BattleTag");
                if (string.IsNullOrEmpty(tag))
                {
                    await ReplyAsync("This user didn't set a battletag");
                    return;
                }

                var profile = await CreateProfile(tag);
                if (profile == null)
                {
                    await ReplyAsync("That player doesn't seem to exist");
                    return;
                }

                await ReplyAsync("", false, profile.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private async Task<EmbedBuilder> CreateProfile(string battletag)
        {
            var owConfig = new OverwatchConfig.Builder().WithPlatforms(Platform.Pc);
            using (var owClient = new OverwatchClient(owConfig))
            {
                var player = await owClient.GetPlayerAsync(battletag);
                if (player.Username == null) return null;
                var eb = new EmbedBuilder();
                eb.WithAuthor(new EmbedAuthorBuilder()
                    .WithIconUrl(player.ProfilePortraitUrl)
                    .WithName(player.Username));
                eb.Url = player.ProfileUrl;
                eb.AddInlineField("Level", player.PlayerLevel);
                eb.AddInlineField("Current Rank",
                    player.CompetitiveRank > 0 ? player.CompetitiveRank.ToString() : "Unranked");

                return eb;
            }
        }
    }
}