using System;
using System.Threading.Tasks;
using System.Web;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Clients;
using Geekbot.net.Lib.ErrorHandling;

namespace Geekbot.net.Commands.Integrations
{
    public class Mal : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IMalClient _malClient;

        public Mal(IMalClient malClient, IErrorHandler errorHandler)
        {
            _malClient = malClient;
            _errorHandler = errorHandler;
        }

        [Command("anime", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Show Info about an Anime.")]
        public async Task SearchAnime([Remainder] [Summary("AnimeName")] string animeName)
        {
            try
            {
                if (_malClient.IsLoggedIn())
                {
                    var anime = await _malClient.GetAnime(animeName);
                    if (anime != null)
                    {
                        var eb = new EmbedBuilder();

                        var description = HttpUtility.HtmlDecode(anime.Synopsis)
                            .Replace("<br />", "")
                            .Replace("[i]", "*")
                            .Replace("[/i]", "*");

                        eb.Title = anime.Title;
                        eb.Description = description;
                        eb.ImageUrl = anime.Image;
                        eb.AddInlineField("Premiered", $"{anime.StartDate}");
                        eb.AddInlineField("Ended", anime.EndDate == "0000-00-00" ? "???" : anime.EndDate);
                        eb.AddInlineField("Status", anime.Status);
                        eb.AddInlineField("Episodes", anime.Episodes);
                        eb.AddInlineField("MAL Score", anime.Score);
                        eb.AddInlineField("Type", anime.Type);
                        eb.AddField("MAL Link", $"https://myanimelist.net/anime/{anime.Id}");

                        await ReplyAsync("", false, eb.Build());
                    }
                    else
                    {
                        await ReplyAsync("No anime found with that name...");
                    }
                }
                else
                {
                    await ReplyAsync(
                        "Unfortunally i'm not connected to MyAnimeList.net, please tell my senpai to connect me");
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("manga", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Show Info about a Manga.")]
        public async Task SearchManga([Remainder] [Summary("MangaName")] string mangaName)
        {
            try
            {
                if (_malClient.IsLoggedIn())
                {
                    var manga = await _malClient.GetManga(mangaName);
                    if (manga != null)
                    {
                        var eb = new EmbedBuilder();

                        var description = HttpUtility.HtmlDecode(manga.Synopsis)
                            .Replace("<br />", "")
                            .Replace("[i]", "*")
                            .Replace("[/i]", "*");

                        eb.Title = manga.Title;
                        eb.Description = description;
                        eb.ImageUrl = manga.Image;
                        eb.AddInlineField("Premiered", $"{manga.StartDate}");
                        eb.AddInlineField("Ended", manga.EndDate == "0000-00-00" ? "???" : manga.EndDate);
                        eb.AddInlineField("Status", manga.Status);
                        eb.AddInlineField("Volumes", manga.Volumes);
                        eb.AddInlineField("Chapters", manga.Chapters);
                        eb.AddInlineField("MAL Score", manga.Score);
                        eb.AddField("MAL Link", $"https://myanimelist.net/manga/{manga.Id}");

                        await ReplyAsync("", false, eb.Build());
                    }
                    else
                    {
                        await ReplyAsync("No manga found with that name...");
                    }
                }
                else
                {
                    await ReplyAsync(
                        "Unfortunally i'm not connected to MyAnimeList.net, please tell my senpai to connect me");
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}