using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.Polyfills;

namespace Geekbot.net.Commands.Utils.Quote
{
    [Group("quote")]
    public class Quote : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly DatabaseContext _database;

        public Quote(IErrorHandler errorHandler, DatabaseContext database)
        {
            _errorHandler = errorHandler;
            _database = database;
        }

        [Command]
        [Summary("Return a random quoute from the database")]
        public async Task GetRandomQuote()
        {
            try
            {
                var s = _database.Quotes.Where(e => e.GuildId.Equals(Context.Guild.Id.AsLong())).ToList();
                
                if (!s.Any())
                {
                    await ReplyAsync("This server doesn't seem to have any quotes yet. You can add a quote with `!quote save @user` or `!quote save <messageId>`");
                    return;
                }

                var random = new Random().Next(s.Count());
                var quote = s[random];
                
                var embed = QuoteBuilder(quote);
                await ReplyAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context, "Whoops, seems like the quote was to edgy to return");
            }
        }

        [Command("save")]
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
                if (lastMessage == null) return;
                
                var quote = CreateQuoteObject(lastMessage);
                _database.Quotes.Add(quote);
                await _database.SaveChangesAsync();

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
                _database.Quotes.Add(quote);
                await _database.SaveChangesAsync();
                
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
        [Summary("Create a quote from the last sent message by @user")]
        public async Task ReturnSpecifiedQuote([Summary("@user")] IUser user)
        {
            try
            {
                var lastMessage = await GetLastMessageByUser(user);
                if (lastMessage == null) return;
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
       [Summary("Remove a quote (required mod permissions)")]
       public async Task RemoveQuote([Summary("quoteId")] int id)
       {
           try
           {
               var quote = _database.Quotes.Where(e => e.GuildId == Context.Guild.Id.AsLong() && e.InternalId == id)?.FirstOrDefault();
               if (quote != null)
               {
                   _database.Quotes.Remove(quote);
                   await _database.SaveChangesAsync();
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
               _errorHandler.HandleCommandException(e, Context, "I couldn't find a quote with that id :disappointed:");
           }
       }

        private async Task<IMessage> GetLastMessageByUser(IUser user)
        {
            try
            {
                var list = Context.Channel.GetMessagesAsync().Flatten();
                await list;
                return list.Result
                    .First(msg => msg.Author.Id == user.Id
                                  && msg.Embeds.Count == 0
                                  && msg.Id != Context.Message.Id
                                  && !msg.Content.ToLower().StartsWith("!"));
            }
            catch
            {
                await ReplyAsync($"No quoteable message have been sent by {user.Username} in this channel");
                return null;
            }
        }

        private EmbedBuilder QuoteBuilder(QuoteModel quote)
        {
            var user = Context.Client.GetUserAsync(quote.UserId.AsUlong()).Result ?? new UserPolyfillDto { Username = "Unknown User" };
            var eb = new EmbedBuilder();
            eb.WithColor(new Color(143, 167, 232));
            eb.Title = $"#{quote.InternalId} | {user.Username} @ {quote.Time.Day}.{quote.Time.Month}.{quote.Time.Year}";
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

            var last = _database.Quotes.Where(e => e.GuildId.Equals(Context.Guild.Id.AsLong()))
                                 .OrderByDescending(e => e.InternalId).FirstOrDefault();
            int internalId = 1;
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