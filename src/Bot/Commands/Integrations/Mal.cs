using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
                    var eb = new EmbedBuilder
                    {
                        Title = anime.Title,
                        Description = anime.Description,
                        ImageUrl = anime.ImageURL
                    };
                    
                    eb.AddInlineField("Premiere", FormatDate(anime.StartDate))
                        .AddInlineField("Ended", anime.Airing ? "-" : FormatDate(anime.EndDate))
                        .AddInlineField("Episodes", anime.Episodes)
                        .AddInlineField("MAL Score", anime.Score)
                        .AddInlineField("Type", anime.Type)
                        .AddField("MAL Link", $"https://myanimelist.net/anime/{anime.MalId}");

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
                    var eb = new EmbedBuilder
                    {
                        Title = manga.Title,
                        Description = manga.Description,
                        ImageUrl = manga.ImageURL
                    };

                    eb.AddInlineField("Premiere", FormatDate(manga.StartDate))
                        .AddInlineField("Ended", manga.Publishing ? "-" : FormatDate(manga.EndDate))
                        .AddInlineField("Volumes", manga.Volumes)
                        .AddInlineField("Chapters", manga.Chapters)
                        .AddInlineField("MAL Score", manga.Score)
                        .AddField("MAL Link", $"https://myanimelist.net/manga/{manga.MalId}");

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