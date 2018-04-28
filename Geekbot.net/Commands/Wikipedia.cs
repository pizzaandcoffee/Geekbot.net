using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Geekbot.net.Lib;
using HtmlAgilityPack;
using WikipediaApi;
using WikipediaApi.Page;

namespace Geekbot.net.Commands
{
    public class Wikipedia : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IWikipediaClient _wikipediaClient;
        
        public Wikipedia(IErrorHandler errorHandler, IWikipediaClient wikipediaClient)
        {
            _errorHandler = errorHandler;
            _wikipediaClient = wikipediaClient;
        }

        [Command("wiki", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Get an article from wikipedia.")]
        public async Task GetPreview([Remainder] [Summary("Article")] string articleName)
        {
            try
            {
                var article = await _wikipediaClient.GetPreview(articleName.Replace(" ", "_"));
                
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
                    Description = article.Extract,
                    ImageUrl = article.Thumbnail?.Source.ToString(),
                    Url = article.ContentUrls.Desktop.Page.ToString()
                };
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
                sb.AppendLine($"**{title}** - {desc}");
            }

            return sb.ToString();
        }
    }
}