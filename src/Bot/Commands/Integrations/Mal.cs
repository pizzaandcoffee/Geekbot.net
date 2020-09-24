using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Discord;
using Discord.Commands;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;
using JikanDotNet;

namespace Geekbot.Bot.Commands.Integrations
{
    public class Mal : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IJikan _client;

        public Mal(IErrorHandler errorHandler)
        {
            _client = new Jikan();
            _errorHandler = errorHandler;
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
                    eb.AddInlineField("Premiered", $"{anime.StartDate.Value.ToShortDateString()}");
                    eb.AddInlineField("Ended", anime.Airing ? "Present" : anime.EndDate.Value.ToShortDateString());
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
                await _errorHandler.HandleCommandException(e, Context);
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
                    eb.AddInlineField("Premiered", $"{manga.StartDate.Value.ToShortDateString()}");
                    eb.AddInlineField("Ended", manga.Publishing ? "Present" : manga.EndDate.Value.ToShortDateString());
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
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}