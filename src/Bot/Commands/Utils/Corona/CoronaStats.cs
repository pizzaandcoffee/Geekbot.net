using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;
using Geekbot.Core.Extensions;

namespace Geekbot.Bot.Commands.Utils.Corona
{
    public class CoronaStats : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        
        public CoronaStats(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("corona", RunMode = RunMode.Async)]
        [Summary("Get the latest worldwide corona statistics")]
        public async Task Summary()
        {
            try
            {
                var summary = await HttpAbstractions.Get<CoronaSummaryDto>(new Uri("https://api.covid19api.com/world/total"));
                var activeCases = summary.TotalConfirmed - (summary.TotalRecovered + summary.TotalDeaths);

                string CalculatePercentage(decimal i) => (i / summary.TotalConfirmed).ToString("#0.##%");
                var activePercent = CalculatePercentage(activeCases);
                var recoveredPercentage = CalculatePercentage(summary.TotalRecovered);
                var deathsPercentage = CalculatePercentage(summary.TotalDeaths);

                var numberFormat = "#,#";
                var totalFormatted = summary.TotalConfirmed.ToString(numberFormat);
                var activeFormatted = activeCases.ToString(numberFormat);
                var recoveredFormatted = summary.TotalRecovered.ToString(numberFormat);
                var deathsFormatted = summary.TotalDeaths.ToString(numberFormat);
                
                var eb = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = "Confirmed Corona Cases",
                        IconUrl = "https://www.redcross.org/content/dam/icons/disasters/virus/Virus-1000x1000-R-Pl.png"
                    },
                    Footer = new EmbedFooterBuilder
                    {
                        Text = "Source: covid19api.com",
                    },
                    Color = Color.Red
                };
                eb.AddField("Total", totalFormatted);
                eb.AddInlineField("Active", $"{activeFormatted} ({activePercent})");
                eb.AddInlineField("Recovered", $"{recoveredFormatted} ({recoveredPercentage})");
                eb.AddInlineField("Deaths", $"{deathsFormatted} ({deathsPercentage})");
                
                await Context.Channel.SendMessageAsync(String.Empty, false, eb.Build());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}