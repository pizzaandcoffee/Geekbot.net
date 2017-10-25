using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Serilog;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    public class Counters : ModuleBase
    {
        private readonly IDatabase _redis;
        private readonly IErrorHandler _errorHandler;

        public Counters(IDatabase redis, IErrorHandler errorHandler)
        {
            _redis = redis;
            _errorHandler = errorHandler;
        }

        [Command("good", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Karma)]
        [Summary("Increase Someones Karma")]
        public async Task Good([Summary("@someone")] IUser user)
        {
            try
            {
                var lastKarmaFromRedis = _redis.HashGet($"{Context.Guild.Id}:KarmaTimeout", Context.User.Id.ToString());
                var lastKarma = ConvertToDateTimeOffset(lastKarmaFromRedis.ToString());
                if (user.Id == Context.User.Id)
                {
                    await ReplyAsync($"Sorry {Context.User.Username}, but you can't lower your own karma");
                }
                else if (TimeoutFinished(lastKarma))
                {
                    await ReplyAsync(
                        $"Sorry {Context.User.Username}, but you have to wait {GetTimeLeft(lastKarma)} before you can give karma again...");
                }
                else
                {
                    var newKarma = _redis.HashIncrement($"{Context.Guild.Id}:Karma", user.Id.ToString());
                    _redis.HashSet($"{Context.Guild.Id}:KarmaTimeout",
                        new HashEntry[] {new HashEntry(Context.User.Id.ToString(), DateTimeOffset.Now.ToString("u"))});

                    var eb = new EmbedBuilder();
                    eb.WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(user.GetAvatarUrl())
                        .WithName(user.Username));

                    eb.WithColor(new Color(138, 219, 146));
                    eb.Title = "Karma Increased";
                    eb.AddInlineField("By", Context.User.Username);
                    eb.AddInlineField("amount", "+1");
                    eb.AddInlineField("Current Karma", newKarma);
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("bad", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Karma)]
        [Summary("Decrease Someones Karma")]
        public async Task Bad([Summary("@someone")] IUser user)
        {
            try
            {
                var lastKarmaFromRedis = _redis.HashGet($"{Context.Guild.Id}:KarmaTimeout", Context.User.Id.ToString());
                var lastKarma = ConvertToDateTimeOffset(lastKarmaFromRedis.ToString());
                if (user.Id == Context.User.Id)
                {
                    await ReplyAsync($"Sorry {Context.User.Username}, but you can't lower your own karma");
                }
                else if (TimeoutFinished(lastKarma))
                {
                    await ReplyAsync(
                        $"Sorry {Context.User.Username}, but you have to wait {GetTimeLeft(lastKarma)} before you can take karma again...");
                }
                else
                {
                    var newKarma = _redis.HashDecrement($"{Context.Guild.Id}:Karma", user.Id.ToString());
                    _redis.HashSet($"{Context.Guild.Id}:KarmaTimeout",
                        new HashEntry[] {new HashEntry(Context.User.Id.ToString(), DateTimeOffset.Now.ToString())});

                    var eb = new EmbedBuilder();
                    eb.WithAuthor(new EmbedAuthorBuilder()
                        .WithIconUrl(user.GetAvatarUrl())
                        .WithName(user.Username));

                    eb.WithColor(new Color(138, 219, 146));
                    eb.Title = "Karma Decreased";
                    eb.AddInlineField("By", Context.User.Username);
                    eb.AddInlineField("amount", "-1");
                    eb.AddInlineField("Current Karma", newKarma);
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private DateTimeOffset ConvertToDateTimeOffset(string dateTimeOffsetString)
        {
            if(string.IsNullOrEmpty(dateTimeOffsetString)) return DateTimeOffset.Now.Subtract(new TimeSpan(7, 18, 0, 0));
            return DateTimeOffset.Parse(dateTimeOffsetString);
        }
        
        private bool TimeoutFinished(DateTimeOffset lastKarma)
        {
            return lastKarma.AddMinutes(3) > DateTimeOffset.Now;
        }
        
        private string GetTimeLeft(DateTimeOffset lastKarma)
        {
            var dt = lastKarma.AddMinutes(3).Subtract(DateTimeOffset.Now);
            return $"{dt.Minutes} Minutes and {dt.Seconds} Seconds";
        }
    }
}