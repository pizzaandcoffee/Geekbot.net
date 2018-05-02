using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Geekbot.net.Commands.Utils.Quote
{
    [Group("quote")]
    public class Quote : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabase _redis;

        public Quote(IDatabase redis, IErrorHandler errorHandler)
        {
            _redis = redis;
            _errorHandler = errorHandler;
        }

        [Command]
        [Remarks(CommandCategories.Quotes)]
        [Summary("Return a random quoute from the database")]
        public async Task GetRandomQuote()
        {
            try
            {
                var randomQuotes = _redis.SetMembers($"{Context.Guild.Id}:Quotes");
                var randomNumber = new Random().Next(randomQuotes.Length - 1);
                var randomQuote = randomQuotes[randomNumber];
                var quote = JsonConvert.DeserializeObject<QuoteObjectDto>(randomQuote);
                var embed = QuoteBuilder(quote, randomNumber + 1);
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
        public async Task SaveQuote([Summary("@user")] IUser user)
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

                var lastMessage = await GetLastMessageByUser(user);
                var quote = CreateQuoteObject(lastMessage);
                var quoteStore = JsonConvert.SerializeObject(quote);
                _redis.SetAdd($"{Context.Guild.Id}:Quotes", quoteStore);
                var embed = QuoteBuilder(quote);
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
        public async Task SaveQuote([Summary("messageId")] ulong messageId)
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

                var quote = CreateQuoteObject(message);
                var quoteStore = JsonConvert.SerializeObject(quote);
                _redis.SetAdd($"{Context.Guild.Id}:Quotes", quoteStore);
                var embed = QuoteBuilder(quote);
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
        public async Task ReturnSpecifiedQuote([Summary("@user")] IUser user)
        {
            try
            {
                var lastMessage = await GetLastMessageByUser(user);
                var quote = CreateQuoteObject(lastMessage);
                var embed = QuoteBuilder(quote);
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
        public async Task ReturnSpecifiedQuote([Summary("messageId")] ulong messageId)
        {
            try
            {
                var message = await Context.Channel.GetMessageAsync(messageId);
                var quote = CreateQuoteObject(message);
                var embed = QuoteBuilder(quote);
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
        public async Task RemoveQuote([Summary("quoteId")] int id)
        {
            try
            {
                var quotes = _redis.SetMembers($"{Context.Guild.Id}:Quotes");
                var success = _redis.SetRemove($"{Context.Guild.Id}:Quotes", quotes[id - 1]);
                if (success)
                {
                    var quote = JsonConvert.DeserializeObject<QuoteObjectDto>(quotes[id - 1]);
                    var embed = QuoteBuilder(quote);
                    await ReplyAsync($"**Removed #{id}**", false, embed.Build());
                } 
                else
                {
                    await ReplyAsync("I couldn't find a quote with that id :disappointed:");
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context,
                    "I couldn't find a quote with that id :disappointed:");
            }
        }

        private async Task<IMessage> GetLastMessageByUser(IUser user)
        {
            var list = Context.Channel.GetMessagesAsync().Flatten();
            await list;
            return list.Result
                .First(msg => msg.Author.Id == user.Id
                              && msg.Embeds.Count == 0
                              && msg.Id != Context.Message.Id
                              && !msg.Content.ToLower().StartsWith("!"));
        }

        private EmbedBuilder QuoteBuilder(QuoteObjectDto quote, int id = 0)
        {
            var user = Context.Client.GetUserAsync(quote.UserId).Result;
            var eb = new EmbedBuilder();
            eb.WithColor(new Color(143, 167, 232));
            eb.Title = id == 0 ? "" : $"#{id} | ";
            eb.Title += $"{user.Username} @ {quote.Time.Day}.{quote.Time.Month}.{quote.Time.Year}";
            eb.Description = quote.Quote;
            eb.ThumbnailUrl = user.GetAvatarUrl();
            if (quote.Image != null) eb.ImageUrl = quote.Image;
            return eb;
        }

        private QuoteObjectDto CreateQuoteObject(IMessage message)
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

            return new QuoteObjectDto
            {
                UserId = message.Author.Id,
                Time = message.Timestamp.DateTime,
                Quote = message.Content,
                Image = image
            };
        }
    }
}