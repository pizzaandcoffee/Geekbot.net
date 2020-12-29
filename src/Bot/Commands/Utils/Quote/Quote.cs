using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.CommandPreconditions;
using Geekbot.Core.Database;
using Geekbot.Core.Database.Models;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.GuildSettingsManager;
using Geekbot.Core.Polyfills;
using Geekbot.Core.RandomNumberGenerator;
using Geekbot.Core.UserRepository;

namespace Geekbot.Bot.Commands.Utils.Quote
{
    [Group("quote")]
    [DisableInDirectMessage]
    public class Quote : GeekbotCommandBase
    {
        private readonly DatabaseContext _database;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly IUserRepository _userRepository;
        private readonly bool _isDev;

        public Quote(IErrorHandler errorHandler, DatabaseContext database, IRandomNumberGenerator randomNumberGenerator, IGuildSettingsManager guildSettingsManager, IUserRepository userRepository)
            : base(errorHandler, guildSettingsManager)
        {
            _database = database;
            _randomNumberGenerator = randomNumberGenerator;
            _userRepository = userRepository;
            // to remove restrictions when developing
            _isDev = Constants.BotVersion() == "0.0.0-DEV";
        }

        [Command]
        [Summary("Return a random quote from the database")]
        public async Task GetRandomQuote()
        {
            try
            {
                var totalQuotes = await _database.Quotes.CountAsync(e => e.GuildId.Equals(Context.Guild.Id.AsLong()));
                
                if (totalQuotes == 0)
                {
                    await ReplyAsync(Localization.Quote.NoQuotesFound);
                    return;
                }

                var random = _randomNumberGenerator.Next(0, totalQuotes - 1);
                var quote = _database.Quotes.Where(e => e.GuildId.Equals(Context.Guild.Id.AsLong())).Skip(random).Take(1);

                var embed = QuoteBuilder(quote.FirstOrDefault());
                await ReplyAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context, "Whoops, seems like the quote was to edgy to return");
            }
        }

        [Command("add")]
        [Alias("save")]
        [Summary("Add a quote from the last sent message by @user")]
        public async Task AddQuote([Summary("@someone")] IUser user)
        {
            await QuoteFromMention(user, true);
        }
        
        [Command("make")]
        [Alias("preview")]
        [Summary("Preview a quote from the last sent message by @user")]
        public async Task ReturnSpecifiedQuote([Summary("@someone")] IUser user)
        {
            await QuoteFromMention(user, false);
        }
        
        [Command("add")]
        [Alias("save")]
        [Summary("Add a quote from a message link")]
        public async Task AddQuote([Summary("message-link")] string messageLink)
        {
            await QuoteFromMessageLink(messageLink, true);
        }
        
        [Command("make")]
        [Alias("preview")]
        [Summary("Preview a quote from a message link")]
        public async Task ReturnSpecifiedQuote([Summary("message-link")] string messageLink)
        {
            await QuoteFromMessageLink(messageLink, false);
        }
        
       [Command("remove")]
       [RequireUserPermission(GuildPermission.ManageMessages)]
       [Summary("Remove a quote (user needs the 'ManageMessages' permission)")]
       public async Task RemoveQuote([Summary("quote-ID")] int id)
       {
           try
           {
               var quote = _database.Quotes.Where(e => e.GuildId == Context.Guild.Id.AsLong() && e.InternalId == id)?.FirstOrDefault();
               if (quote != null)
               {
                   _database.Quotes.Remove(quote);
                   await _database.SaveChangesAsync();
                   var embed = QuoteBuilder(quote);
                   await ReplyAsync(string.Format(Localization.Quote.Removed, id), false, embed.Build());
               }
               else
               {
                   await ReplyAsync(Localization.Quote.NotFoundWithId);
               }
           }
           catch (Exception e)
           {
               await ErrorHandler.HandleCommandException(e, Context, "I couldn't find a quote with that id :disappointed:");
           }
       }

       [Command("stats")]
       [Summary("Show quote stats for this server")]
       public async Task GetQuoteStatsForServer()
       {
           try
           {
               // setup
               var eb = new EmbedBuilder();
               eb.Author = new EmbedAuthorBuilder()
               {
                   IconUrl = Context.Guild.IconUrl,
                   Name = $"{Context.Guild.Name} - {Localization.Quote.QuoteStats}"
               };

               // gather data
               var totalQuotes = _database.Quotes.Count(row => row.GuildId == Context.Guild.Id.AsLong());
               if (totalQuotes == 0)
               {
                   // no quotes, no stats, end of the road
                   await ReplyAsync(Localization.Quote.NoQuotesFound);
                   return;
               }
               
               var mostQuotedPerson = _database.Quotes
                   .Where(row => row.GuildId == Context.Guild.Id.AsLong())
                   .GroupBy(row => row.UserId)
                   .Select(row => new { userId = row.Key, amount = row.Count()})
                   .OrderBy(row => row.amount)
                   .Last();
               var mostQuotedPersonUser = Context.Client.GetUserAsync(mostQuotedPerson.userId.AsUlong()).Result ?? new UserPolyfillDto {Username = "Unknown User"};

               var quotesByYear = _database.Quotes
                   .Where(row => row.GuildId == Context.Guild.Id.AsLong())
                   .GroupBy(row => row.Time.Year)
                   .Select(row => new { year = row.Key, amount = row.Count()})
                   .OrderBy(row => row.year);
               
               // add data to the embed
               eb.AddField(Localization.Quote.MostQuotesPerson, $"{mostQuotedPersonUser.Username} ({mostQuotedPerson.amount})");
               eb.AddInlineField(Localization.Quote.TotalQuotes, totalQuotes);

               foreach (var year in quotesByYear)
               {
                   eb.AddInlineField(year.year.ToString(), year.amount);
               }

               await ReplyAsync("", false, eb.Build());
           }
           catch (Exception e)
           {
               await ErrorHandler.HandleCommandException(e, Context);
           }
       }

       private async Task QuoteFromMention(IUser user, bool saveToDb)
        {
            try
            {
                var list = Context.Channel.GetMessagesAsync().Flatten();
                var message = await list.FirstOrDefaultAsync(msg => 
                    msg.Author.Id == user.Id &&
                    msg.Embeds.Count == 0 &&
                    msg.Id != Context.Message.Id &&
                    !msg.Content.ToLower().StartsWith("!"));
                if (message == null) return;
            
                await ProcessQuote(message, saveToDb);
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context, $"No quoteable messages have been sent by {user.Username} in this channel");
            }
            
        }

       private async Task QuoteFromMessageLink(string messageLink, bool saveToDb)
        {
            try
            {
                if (!MessageLink.IsValid(messageLink))
                {
                    await ReplyAsync(Localization.Quote.NotAValidMessageLink);
                    return;
                }

                var link = new MessageLink(messageLink);
                if (link.GuildId != Context.Guild.Id)
                {
                    await ReplyAsync(Localization.Quote.OnlyQuoteFromSameServer);
                    return;
                }

                var channel = link.ChannelId == Context.Channel.Id
                    ? Context.Channel
                    : await Context.Guild.GetTextChannelAsync(link.ChannelId);

                var message = await channel.GetMessageAsync(link.MessageId);

                await ProcessQuote(message, saveToDb);
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context, "I couldn't find that message :disappointed:");
            }
        }

        private async Task ProcessQuote(IMessage message, bool saveToDb)
        {
            if (message.Author.Id == Context.Message.Author.Id && saveToDb && !_isDev)
            {
                await ReplyAsync(Localization.Quote.CannotSaveOwnQuotes);
                return;
            }
                
            if (message.Author.IsBot && saveToDb  && !_isDev)
            {
                await ReplyAsync(Localization.Quote.CannotQuoteBots);
                return;
            }

            var quote = CreateQuoteObject(message);
            if (saveToDb)
            {
                await _database.Quotes.AddAsync(quote);
                await _database.SaveChangesAsync();
            }

            var embed = QuoteBuilder(quote);
            
            var sb = new StringBuilder();
            if (saveToDb) sb.AppendLine(Localization.Quote.QuoteAdded);
            
            await ReplyAsync(sb.ToString(), false, embed.Build());
        }

        private EmbedBuilder QuoteBuilder(QuoteModel quote)
        {
            var user = Context.Client.GetUserAsync(quote.UserId.AsUlong()).Result;
            if (user == null)
            {
                var fallbackUserFromRepo = _userRepository.Get(quote.UserId.AsUlong());
                user = new UserPolyfillDto()
                {
                    Username = fallbackUserFromRepo?.Username ?? "Unknown User",
                    AvatarUrl = fallbackUserFromRepo?.AvatarUrl
                };
            }
            var eb = new EmbedBuilder();
            eb.WithColor(new Color(143, 167, 232));
            if (quote.InternalId == 0)
            {
                eb.Title = $"{user.Username} @ {quote.Time.Day}.{quote.Time.Month}.{quote.Time.Year}";                
            }
            else
            {
                eb.Title = $"#{quote.InternalId} | {user.Username} @ {quote.Time.Day}.{quote.Time.Month}.{quote.Time.Year}";                
            }
            eb.Description = quote.Quote;
            eb.ThumbnailUrl = user.GetAvatarUrl();
            if (quote.Image != null) eb.ImageUrl = quote.Image;
            return eb;
        }

        private QuoteModel CreateQuoteObject(IMessage message)
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

            var last = _database.Quotes.Where(e => e.GuildId.Equals(Context.Guild.Id.AsLong())).OrderByDescending(e => e.InternalId).FirstOrDefault();
            var internalId = 0;
            if (last != null) internalId = last.InternalId + 1;
            return new QuoteModel()
            {
                InternalId = internalId,
                GuildId = Context.Guild.Id.AsLong(),
                UserId = message.Author.Id.AsLong(),
                Time = message.Timestamp.DateTime,
                Quote = message.Content,
                Image = image
            };
        }
    }
}