using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task Summary([Summary("CountryCode")] string countryCode = null)
        {
            try
            {
                var summary = await GetCoronaInfo(countryCode);
                if (summary == null)
                {
                    await Context.Channel.SendMessageAsync($"`{countryCode}` is not a valid country code");
                    return;
                }
                
                var activeCases = summary.Cases - (summary.Recovered + summary.Deaths);

                string CalculatePercentage(decimal i) => (i / summary.Cases).ToString("#0.##%");
                var activePercent = CalculatePercentage(activeCases);
                var recoveredPercentage = CalculatePercentage(summary.Recovered);
                var deathsPercentage = CalculatePercentage(summary.Deaths);

                var numberFormat = "#,#";
                var totalFormatted = summary.Cases.ToString(numberFormat);
                var activeFormatted = activeCases.ToString(numberFormat);
                var recoveredFormatted = summary.Recovered.ToString(numberFormat);
                var deathsFormatted = summary.Deaths.ToString(numberFormat);

                var eb = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = "Confirmed Corona Cases",
                        IconUrl = "https://www.redcross.org/content/dam/icons/disasters/virus/Virus-1000x1000-R-Pl.png"
                    },
                    Footer = new EmbedFooterBuilder
                    {
                        Text = "Source: covid19-api.org",
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

        private async Task<CoronaTotalDto> GetCoronaInfo(string countryCode = null)
        {
            var allCountries = await HttpAbstractions.Get<List<CoronaApiCountryResponseDto>>(new Uri("https://covid19-api.org/api/status"));
            
            if (string.IsNullOrEmpty(countryCode))
            {
                return allCountries.Aggregate(
                    new CoronaTotalDto(),
                    (accumulate, source) =>
                    {
                        accumulate.Cases += source.Cases;
                        accumulate.Deaths += source.Deaths;
                        accumulate.Recovered += source.Recovered;
                        return accumulate;
                    }
                );
            }

            if (countryCode.Length != 2)
            {
                return null;
            }

            var upcasedCountryCode = countryCode.ToUpper();
            var countryStats = allCountries.Find(x => x.Country == upcasedCountryCode);
            if (countryStats == null)
            {
                return null;
            }

            return new CoronaTotalDto()
            {
                Cases = countryStats.Cases,
                Deaths = countryStats.Deaths,
                Recovered = countryStats.Recovered,
            };
        }
    }
}