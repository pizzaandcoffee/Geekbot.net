using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib;
using Newtonsoft.Json;

namespace Geekbot.net.Commands
{
    public class Changelog : ModuleBase
    {
        private readonly DiscordSocketClient _client;
        private readonly IErrorHandler _errorHandler;

        public Changelog(IErrorHandler errorHandler, DiscordSocketClient client)
        {
            _errorHandler = errorHandler;
            _client = client;
        }

        [Command("changelog", RunMode = RunMode.Async)]
        [Alias("updates")]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Show the latest 5 updates")]
        public async Task GetChangelog()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.github.com");
                    client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                        "http://developer.github.com/v3/#user-agent-required");
                    var response = await client.GetAsync("/repos/pizzaandcoffee/geekbot.net/commits");
                    response.EnsureSuccessStatusCode();

                    var stringResponse = await response.Content.ReadAsStringAsync();
                    var commits = JsonConvert.DeserializeObject<List<CommitDto>>(stringResponse);
                    var eb = new EmbedBuilder();
                    eb.WithColor(new Color(143, 165, 102));
                    eb.WithAuthor(new EmbedAuthorBuilder
                    {
                        IconUrl = _client.CurrentUser.GetAvatarUrl(),
                        Name = "Latest Updates",
                        Url = "https://geekbot.pizzaandcoffee.rocks/updates"
                    });
                    var sb = new StringBuilder();
                    foreach (var commit in commits.Take(10))
                        sb.AppendLine($"- {commit.Commit.Message} ({commit.Commit.Author.Date:yyyy-MM-dd})");
                    eb.Description = sb.ToString();
                    eb.WithFooter(new EmbedFooterBuilder
                    {
                        Text = $"List generated from github commits on {DateTime.Now:yyyy-MM-dd}"
                    });
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private class CommitDto
        {
            public CommitInfo Commit { get; set; }
        }

        private class CommitInfo
        {
            public CommitAuthor Author { get; set; }
            public string Message { get; set; }
        }

        private class CommitAuthor
        {
            public DateTimeOffset Date { get; set; }
        }
    }
}