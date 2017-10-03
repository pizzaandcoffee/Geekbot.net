using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    [Group("battletag")]
    public class BattleTag : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabase _redis;
        private readonly IUserRepository _userRepository;
        
        public BattleTag(IErrorHandler errorHandler, IDatabase redis, IUserRepository userRepository)
        {
            _errorHandler = errorHandler;
            _redis = redis;
            _userRepository = userRepository;
        }

        [Command(RunMode = RunMode.Async)]
        [Summary("Get your battletag")]
        public async Task BattleTagCmd()
        {
            try
            {
                var tag = _userRepository.getUserSetting(Context.User.Id, "BattleTag");
                if (!string.IsNullOrEmpty(tag))
                {
                    
                    await ReplyAsync($"Your BattleTag is {tag}");
                }
                else
                {
                    await ReplyAsync("You haven't set your BattleTag, set it with `!battletag user#1234`");
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
        
        [Command(RunMode = RunMode.Async)]
        [Summary("Save your battletag")]
        public async Task BattleTagCmd([Summary("Battletag")] string tag)
        {
            try
            {
                if (isValidTag(tag))
                {
                    _userRepository.saveUserSetting(Context.User.Id, "BattleTag", tag);
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

        private bool isValidTag(string tag)
        {
            var splited = tag.Split("#");
            if (splited.Length != 2) return false;
            if (!int.TryParse(splited[1], out int discriminator)) return false;
            if (splited[1].Length == 4 || splited[1].Length == 5) return true;
            return false;
        }
    }
}