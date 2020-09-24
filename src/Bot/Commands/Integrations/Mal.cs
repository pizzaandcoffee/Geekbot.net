using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Discord;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using Geekbot.Core.GuildSettingsManager;
using JikanDotNet;

namespace Geekbot.Bot.Commands.Integrations
{
    public class Mal : GeekbotCommandBase
    {
        private readonly IJikan _client;

        public Mal(IErrorHandler errorHandler, IGuildSettingsManager guildSettingsManager) : base(errorHandler, guildSettingsManager)
        {
            _client = new Jikan();
        }

        [Command("anime", RunMode = RunMode.Async)]
        [Summary("Show Info about an Anime.")]
        public async Task SearchAnime([Remainder] [Summary("anime-name")] string animeName)
        {
            try
            {
                var results = await _client.SearchAnime(animeName);
                var anime = results.Results.FirstOrDefault();
                if (anime != null)
                {
                    var eb = new EmbedBuilder();

                    var description = HttpUtility.HtmlDecode(anime.Description)
                        .Replace("<br />", "")
                        .Replace("[i]", "*")
                        .Replace("[/i]", "*");

                    eb.Title = anime.Title;
                    eb.Description = description;
                    eb.ImageUrl = anime.ImageURL;
                    eb.AddInlineField("Premiered", FormatDate(anime.StartDate));
                    eb.AddInlineField("Ended", anime.Airing ? "Present" : FormatDate(anime.EndDate));
                    eb.AddInlineField("Episodes", anime.Episodes);
                    eb.AddInlineField("MAL Score", anime.Score);
                    eb.AddInlineField("Type", anime.Type);
                    eb.AddField("MAL Link", $"https://myanimelist.net/anime/{anime.MalId}");

                    await ReplyAsync("", false, eb.Build());
                }
                else
                {
                    await ReplyAsync("No anime found with that name...");
                }
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("manga", RunMode = RunMode.Async)]
        [Summary("Show Info about a Manga.")]
        public async Task SearchManga([Remainder] [Summary("manga-name")] string mangaName)
        {
            try
            {
                var results = await _client.SearchManga(mangaName);
                var manga = results.Results.FirstOrDefault();
                if (manga != null)
                {
                    var eb = new EmbedBuilder();
    
                    var description = HttpUtility.HtmlDecode(manga.Description)
                        .Replace("<br />", "")
                        .Replace("[i]", "*")
                        .Replace("[/i]", "*");
    
                    eb.Title = manga.Title;
                    eb.Description = description;
                    eb.ImageUrl = manga.ImageURL;
                    eb.AddInlineField("Premiered", FormatDate(manga.StartDate));
                    eb.AddInlineField("Ended", manga.Publishing ? "Present" : FormatDate(manga.EndDate));
                    eb.AddInlineField("Volumes", manga.Volumes);
                    eb.AddInlineField("Chapters", manga.Chapters);
                    eb.AddInlineField("MAL Score", manga.Score);
                    eb.AddField("MAL Link", $"https://myanimelist.net/manga/{manga.MalId}");
    
                    await ReplyAsync("", false, eb.Build());
                }
                else
                {
                    await ReplyAsync("No manga found with that name...");
                }
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }

        private string FormatDate(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
            {
                return DateTime.MinValue.ToString("d", Thread.CurrentThread.CurrentUICulture);
            }
            
            return dateTime.Value.ToString("d", Thread.CurrentThread.CurrentUICulture);
        }
    }
}