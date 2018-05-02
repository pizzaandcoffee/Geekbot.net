using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.UserRepository;

namespace Geekbot.net.Commands.Games
{
    [Group("battletag")]
    public class BattleTag : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IUserRepository _userRepository;

        public BattleTag(IErrorHandler errorHandler, IUserRepository userRepository)
        {
            _errorHandler = errorHandler;
            _userRepository = userRepository;
        }

        [Command(RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Games)]
        [Summary("Get your battletag")]
        public async Task BattleTagCmd()
        {
            try
            {
                var tag = _userRepository.GetUserSetting(Context.User.Id, "BattleTag");
                if (!string.IsNullOrEmpty(tag))
                    await ReplyAsync($"Your BattleTag is {tag}");
                else
                    await ReplyAsync("You haven't set your BattleTag, set it with `!battletag user#1234`");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command(RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Games)]
        [Summary("Save your battletag")]
        public async Task BattleTagCmd([Summary("Battletag")] string tag)
        {
            try
            {
                if (IsValidTag(tag))
                {
                    _userRepository.SaveUserSetting(Context.User.Id, "BattleTag", tag);
                    await ReplyAsync("Saved!");
                }
                else
                {
                    await ReplyAsync("That doesn't seem to be a valid battletag");
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        public static bool IsValidTag(string tag)
        {
            var splited = tag.Split("#");
            if (splited.Length != 2) return false;
            if (!int.TryParse(splited[1], out var discriminator)) return false;
            return splited[1].Length == 4 || splited[1].Length == 5;
        }
    }
}