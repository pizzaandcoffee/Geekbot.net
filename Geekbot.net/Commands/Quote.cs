using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    [Group("quote")]
    public class Quote : ModuleBase
    {
        private readonly IDatabase redis;
        private readonly ILogger logger;
        private readonly IErrorHandler errorHandler;
        
        public Quote(IDatabase redis, ILogger logger, IErrorHandler errorHandler)
        {
            this.redis = redis;
            this.logger = logger;
            this.errorHandler = errorHandler;
        }

        [Command()]
        [Summary("Return a random quoute from the database")]
        public async Task getRandomQuote()
        {
            var randomQuote = redis.SetRandomMember($"{Context.Guild.Id}:Quotes");          
            try
            {
                var quote = JsonConvert.DeserializeObject<QuoteObject>(randomQuote);
                var embed = quoteBuilder(quote);
                await ReplyAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                errorHandler.HandleCommandException(e, Context, "Whoops, seems like the quote was to edgy to return");
            }
        }
        
        [Command("save")]
        [Summary("Save a quote from the last sent message by @user")]
        public async Task saveQuote([Summary("@user")] IUser user)
        {
            try
            {
                var lastMessage = await getLastMessageByUser(user);
                var quote = createQuoteObject(lastMessage);
                var quoteStore = JsonConvert.SerializeObject(quote);
                redis.SetAdd($"{Context.Guild.Id}:Quotes", quoteStore);
                var embed = quoteBuilder(quote);
                await ReplyAsync("**Quote Added**", false, embed.Build());
            }
            catch (Exception e)
            {
                errorHandler.HandleCommandException(e, Context, "I counldn't find a quote from that user :disappointed:");
            }
        }
        
        [Command("save")]
        [Summary("Save a quote from a message id")]
        public async Task saveQuote([Summary("messageId")] ulong messageId)
        {
            try
            {
                var message = await Context.Channel.GetMessageAsync(messageId);
                var quote = createQuoteObject(message);
                var quoteStore = JsonConvert.SerializeObject(quote);
                redis.SetAdd($"{Context.Guild.Id}:Quotes", quoteStore);
                var embed = quoteBuilder(quote);
                await ReplyAsync("**Quote Added**", false, embed.Build());
                
            }
            catch (Exception e)
            {
                errorHandler.HandleCommandException(e, Context, "I couldn't find a message with that id :disappointed:");
            }
        }
        
        [Command("make")]
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
                errorHandler.HandleCommandException(e, Context, "I counldn't find a quote from that user :disappointed:");
            }
        }
        
        [Command("make")]
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
                errorHandler.HandleCommandException(e, Context, "I couldn't find a message with that id :disappointed:");
            }
        }

        private async Task<IMessage> getLastMessageByUser(IUser user)
        {
            Task<IEnumerable<IMessage>> list = Context.Channel.GetMessagesAsync().Flatten();
            await list;
            return list.Result
                .First(msg => msg.Author.Id == user.Id 
                    && msg.Embeds.Count == 0 
                    && msg.Id != Context.Message.Id
                    && !msg.Content.ToLower().StartsWith("!"));
        }
        
        private EmbedBuilder quoteBuilder(QuoteObject quote)
        {
            var user = Context.Client.GetUserAsync(quote.userId).Result;
            var eb = new EmbedBuilder();
            eb.WithColor(new Color(143, 167, 232));
            eb.Title = $"{user.Username} @ {quote.time.Day}.{quote.time.Month}.{quote.time.Year}";
            eb.Description = quote.quote;
            eb.ThumbnailUrl = user.GetAvatarUrl();
            if (quote.image != null)
            {
                eb.ImageUrl = quote.image;
            }
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
            return new QuoteObject()
            {
                userId = message.Author.Id,
                time = message.Timestamp.DateTime,
                quote = message.Content,
                image = image
            };
        }
    }
    
    public class QuoteObject {
        public ulong userId { get; set; }
        public string quote { get; set; }
        public DateTime time { get; set; }
        public string image { get; set; }
    }
}