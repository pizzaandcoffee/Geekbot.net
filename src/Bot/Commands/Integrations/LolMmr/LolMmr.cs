using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Discord.Commands;
using Geekbot.Core;
using Geekbot.Core.ErrorHandling;

namespace Geekbot.Bot.Commands.Integrations.LolMmr
{
    public class LolMmr : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public LolMmr(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("mmr", RunMode = RunMode.Async)]
        [Summary("Get the League of Legends MMR for a specified summoner")]
        public async Task GetMmr([Remainder] [Summary("summoner")] string summonerName)
        {
            try
            {
                LolMmrDto data;
                try
                {
                    var name = HttpUtility.UrlEncode(summonerName.ToLower());
                    var httpClient = HttpAbstractions.CreateDefaultClient();
                    // setting the user agent in accordance with the whatismymmr.com api rules
                    httpClient.DefaultRequestHeaders.Remove("User-Agent");
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Linux:rocks.pizzaandcoffee.geekbot:v0.0.0");
                    data = await HttpAbstractions.Get<LolMmrDto>(new Uri($"https://euw.whatismymmr.com/api/v1/summoner?name={name}"), httpClient);
                }
                catch (HttpRequestException e)
                {
                    if (e.StatusCode != HttpStatusCode.NotFound) throw e;
                    
                    await Context.Channel.SendMessageAsync("Player not found");
                    return;

                }

                var sb = new StringBuilder();
                sb.AppendLine($"**MMR for {summonerName}**");
                sb.AppendLine($"Normal: {data.Normal.Avg}");
                sb.AppendLine($"Ranked: {data.Ranked.Avg}");
                sb.AppendLine($"ARAM: {data.ARAM.Avg}");
                
                await Context.Channel.SendMessageAsync(sb.ToString());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}