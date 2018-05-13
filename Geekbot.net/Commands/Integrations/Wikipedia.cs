using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;
using HtmlAgilityPack;
using StackExchange.Redis;
using WikipediaApi;
using WikipediaApi.Page;

namespace Geekbot.net.Commands.Integrations
{
    public class Wikipedia : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IWikipediaClient _wikipediaClient;
        private readonly IDatabase _redis;

        public Wikipedia(IErrorHandler errorHandler, IWikipediaClient wikipediaClient, IDatabase redis)
        {
            _errorHandler = errorHandler;
            _wikipediaClient = wikipediaClient;
            _redis = redis;
        }

        [Command("wiki", RunMode = RunMode.Async)]
        [Summary("Get an article from wikipedia.")]
        public async Task GetPreview([Remainder] [Summary("Article")] string articleName)
        {
            try
            {
                var wikiLang = _redis.HashGet($"{Context.Guild.Id}:Settings", "WikiLang").ToString();
                if (string.IsNullOrEmpty(wikiLang))
                {
                    wikiLang = "en";
                }
                var article = await _wikipediaClient.GetPreview(articleName.Replace(" ", "_"), wikiLang);
                
                if (article.Type != PageTypes.Standard)
                {
                    switch (article.Type)
                    {
                        case PageTypes.Disambiguation:
                            await ReplyAsync($"**__Disambiguation__**\r\n{DisambiguationExtractor(article.ExtractHtml)}");
                            break;
                        case PageTypes.MainPage:
                            await ReplyAsync("The main page is not supported");
                            break;
                        case PageTypes.NoExtract:
                            await ReplyAsync($"This page has no summary, here is the link: {article.ContentUrls.Desktop.Page}");
                            break;
                        case PageTypes.Standard:
                            break;
                        default:
                            await ReplyAsync($"This page type is currently not supported, here is the link: {article.ContentUrls.Desktop.Page}");
                            break;
                    }
                    return;
                }

                var eb = new EmbedBuilder
                {
                    Title = article.Title,
                    Description = article.Description,
                    ImageUrl = article.Thumbnail?.Source.ToString(),
                    Url = article.ContentUrls.Desktop.Page.ToString(),
                    Color = new Color(246,246,246),
                    Timestamp = article.Timestamp,
                    Footer = new EmbedFooterBuilder
                    {
                        Text = "Last Edit",
                        IconUrl = "http://icons.iconarchive.com/icons/sykonist/popular-sites/256/Wikipedia-icon.png"
                    }
                };
                
                eb.AddField("Description", article.Extract);
                if (article.Coordinates != null) eb.AddField("Coordinates", $"{article.Coordinates.Lat} Lat {article.Coordinates.Lon} Lon");
                await ReplyAsync("", false, eb.Build());
            }
            catch (HttpRequestException)
            {
                await ReplyAsync("I couldn't find that article");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private string DisambiguationExtractor(string extractHtml)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(extractHtml);
            var nodes = doc.DocumentNode.SelectNodes("//li");
            if (nodes == null) return "(List is to long to show)";
            var sb = new StringBuilder();
            foreach (var node in nodes)
            {
                var split = node.InnerText.Split(',');
                var title = split.First();
                var desc = string.Join(",", split.Skip(1));
                sb.AppendLine($"• **{title}** -{desc}");
            }

            return sb.ToString();
        }
    }
}