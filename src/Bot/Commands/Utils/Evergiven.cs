using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.GuildSettingsManager;
using HtmlAgilityPack;

namespace Geekbot.Bot.Commands.Utils
{
    public class Evergiven : GeekbotCommandBase
    {
        public Evergiven(IErrorHandler errorHandler, IGuildSettingsManager guildSettingsManager) : base(errorHandler, guildSettingsManager)
        {
        }

        [Command("evergiven", RunMode = RunMode.Async)]
        [Summary("Check if the evergiven ship is still stuck in the suez canal")]
        public async Task GetStatus()
        {
            try
            {
                var httpClient = HttpAbstractions.CreateDefaultClient();
                var response = await httpClient.GetAsync("https://istheshipstillstuck.com/");
                response.EnsureSuccessStatusCode();
                var stringResponse = await response.Content.ReadAsStringAsync();
                
                var doc = new HtmlDocument();
                doc.LoadHtml(stringResponse);
                var statusNode = doc.DocumentNode.SelectNodes("//a").FirstOrDefault();

                if (statusNode == null)
                {
                    await ReplyAsync("Maybe, check <https://istheshipstillstuck.com/>");
                    return;
                }

                var sb = new StringBuilder();

                sb.Append($"Is that ship still stuck? {statusNode.InnerHtml}");
                if (statusNode.Attributes.Contains("href"))
                {
                    sb.Append($" {statusNode.Attributes["href"].Value}");
                }
                
                var stuckTimer = doc.DocumentNode.SelectNodes("//p")?.First(node => node.Attributes.First(attr => attr.Name == "style")?.Value == "text-align:center");
                if (stuckTimer != null)
                {
                    sb.AppendLine();
                    sb.AppendLine(HttpUtility.HtmlDecode(stuckTimer.InnerText));
                }
                
                await ReplyAsync(sb.ToString());
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
    }
}