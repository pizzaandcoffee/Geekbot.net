using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    [Group("quote")]
    public class Quote : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabase _redis;

        public Quote(IDatabase redis, IErrorHandler errorHandler, Random random)
        {
            _redis = redis;
            _errorHandler = errorHandler;
        }

        [Command]
        [Remarks(CommandCategories.Quotes)]
        [Summary("Return a random quoute from the database")]
        public async Task getRandomQuote()
        {
            try
            {
                var randomQuotes = _redis.SetMembers($"{Context.Guild.Id}:Quotes");
                var randomNumber = new Random().Next(randomQuotes.Length - 1);
                var randomQuote = randomQuotes[randomNumber];
                var quote = JsonConvert.DeserializeObject<QuoteObject>(randomQuote);
                var embed = quoteBuilder(quote, randomNumber + 1);
                await ReplyAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context, "Whoops, seems like the quote was to edgy to return");
            }
        }

        [Command("save")]
        [Remarks(CommandCategories.Quotes)]
        [Summary("Save a quote from the last sent message by @user")]
        public async Task saveQuote([Summary("@user")] IUser user)
        {
            try
            {
                if (user.Id == Context.Message.Author.Id)
                {
                    await ReplyAsync("You can't save your own quotes...");
                    return;
                }

                if (user.IsBot)
                {
                    await ReplyAsync("You can't save quotes by a bot...");
                    return;
                }

                var lastMessage = await getLastMessageByUser(user);
                var quote = createQuoteObject(lastMessage);
                var quoteStore = JsonConvert.SerializeObject(quote);
                _redis.SetAdd($"{Context.Guild.Id}:Quotes", quoteStore);
                var embed = quoteBuilder(quote);
                await ReplyAsync("**Quote Added**", false, embed.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context,
                    "I counldn't find a quote from that user :disappointed:");
            }
        }

        [Command("save")]
        [Remarks(CommandCategories.Quotes)]
        [Summary("Save a quote from a message id")]
        public async Task saveQuote([Summary("messageId")] ulong messageId)
        {
            try
            {
                var message = await Context.Channel.GetMessageAsync(messageId);
                if (message.Author.Id == Context.Message.Author.Id)
                {
                    await ReplyAsync("You can't save your own quotes...");
                    return;
                }

                if (message.Author.IsBot)
                {
                    await ReplyAsync("You can't save quotes by a bot...");
                    return;
                }

                var quote = createQuoteObject(message);
                var quoteStore = JsonConvert.SerializeObject(quote);
                _redis.SetAdd($"{Context.Guild.Id}:Quotes", quoteStore);
                var embed = quoteBuilder(quote);
                await ReplyAsync("**Quote Added**", false, embed.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context,
                    "I couldn't find a message with that id :disappointed:");
            }
        }

        [Command("make")]
        [Remarks(CommandCategories.Quotes)]
        [Summary("Create a quote from the last sent message by @user")]
        public async Task returnSpecifiedQuote([Summary("@user")] IUser user)
        {
            try
            {
                var lastMessage = await getLastMessageByUser(user);
                var quote = createQuoteObject(lastMessage);
                var embed = quoteBuilder(quote);
                await ReplyAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context,
                    "I counldn't find a quote from that user :disappointed:");
            }
        }

        [Command("make")]
        [Remarks(CommandCategories.Quotes)]
        [Summary("Create a quote from a message id")]
        public async Task returnSpecifiedQuote([Summary("messageId")] ulong messageId)
        {
            try
            {
                var message = await Context.Channel.GetMessageAsync(messageId);
                var quote = createQuoteObject(message);
                var embed = quoteBuilder(quote);
                await ReplyAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context,
                    "I couldn't find a message with that id :disappointed:");
            }
        }
        
        [Command("remove")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Remarks(CommandCategories.Quotes)]
        [Summary("Remove a quote (required mod permissions)")]
        public async Task removeQuote([Summary("quoteId")] int id)
        {
            try
            {
                var quotes = _redis.SetMembers($"{Context.Guild.Id}:Quotes");
                var success = _redis.SetRemove($"{Context.Guild.Id}:Quotes", quotes[id - 1]);
                if (success)
                {
                    await ReplyAsync($"Removed quote #{id}");
                } 
                else
                {
                    await ReplyAsync($"I couldn't find a quote with that id :disappointed:");
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context,
                    "I couldn't find a quote with that id :disappointed:");
            }
        }

        private async Task<IMessage> getLastMessageByUser(IUser user)
        {
            var list = Context.Channel.GetMessagesAsync().Flatten();
            await list;
            return list.Result
                .First(msg => msg.Author.Id == user.Id
                              && msg.Embeds.Count == 0
                              && msg.Id != Context.Message.Id
                              && !msg.Content.ToLower().StartsWith("!"));
        }

        private EmbedBuilder quoteBuilder(QuoteObject quote, int id = 0)
        {
            var user = Context.Client.GetUserAsync(quote.userId).Result;
            var eb = new EmbedBuilder();
            eb.WithColor(new Color(143, 167, 232));
            eb.Title = id == 0 ? "" : $"#{id} | ";
            eb.Title += $"{user.Username} @ {quote.time.Day}.{quote.time.Month}.{quote.time.Year}";
            eb.Description = quote.quote;
            eb.ThumbnailUrl = user.GetAvatarUrl();
            if (quote.image != null) eb.ImageUrl = quote.image;
            return eb;
        }

        private QuoteObject createQuoteObject(IMessage message)
        {
            string image;
            try
            {
                image = message.Attachments.First().Url;
            }
            catch (Exception)
            {
                image = null;
            }

            return new QuoteObject
            {
                userId = message.Author.Id,
                time = message.Timestamp.DateTime,
                quote = message.Content,
                image = image
            };
        }
    }

    public class QuoteObject
    {
        public ulong userId { get; set; }
        public string quote { get; set; }
        public DateTime time { get; set; }
        public string image { get; set; }
    }
}