using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Bot.Utils;
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
                var yesOrNoNode = doc.DocumentNode.SelectNodes("//a").FirstOrDefault();

                if (yesOrNoNode?.InnerHtml == "Yes.")
                {
                    var stuckSince = DateTime.Now - new DateTime(2021, 03, 23, 10, 39, 0);
                    var formatted = DateLocalization.FormatDateTimeAsRemaining(stuckSince);
                    await ReplyAsync(string.Format(Localization.Evergiven.StillStuck, formatted));// $"Ever Given is **still stuck** in the suez canal! It has been stuck for {formatted}");
                    return;
                }

                await ReplyAsync(Localization.Evergiven.NotStuckAnymore);
            }
            catch (Exception e)
            {
                await ErrorHandler.HandleCommandException(e, Context);
            }
        }
    }
}