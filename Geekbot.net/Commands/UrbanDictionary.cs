using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Newtonsoft.Json;

namespace Geekbot.net.Commands
{
    public class UrbanDictionary : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        
        public UrbanDictionary(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }
        
        [Command("urban", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Lookup something on urban dictionary")]
        public async Task urbanDefine([Remainder, Summary("word")] string word)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.urbandictionary.com");
                    var response = await client.GetAsync($"/v0/define?term={word}");
                    response.EnsureSuccessStatusCode();

                    var stringResponse = await response.Content.ReadAsStringAsync();
                    var definitions = JsonConvert.DeserializeObject<UrbanResponse>(stringResponse);
                    if (definitions.list.Count == 0)
                    {
                        await ReplyAsync("That word is not defined...");
                        return;
                    }
                    var definition = definitions.list.OrderBy(e => e.thumbs_up).First();
                    
                    var eb = new EmbedBuilder();
                    eb.WithAuthor(new EmbedAuthorBuilder()
                    {
                        Name = definition.word,
                        Url = definition.permalink
                    });
                    eb.WithColor(new Color(239,255,0));
                    eb.Description = definition.definition;
                    eb.AddField("Example", definition.example);
                    eb.AddInlineField("Upvotes", definition.thumbs_up);
                    eb.AddInlineField("Downvotes", definition.thumbs_down);
                    eb.AddField("Tags", string.Join(", ", definitions.tags));
                    
                    await ReplyAsync("", false, eb.Build());
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
        
        private class UrbanResponse
        {
            public string[] tags { get; set; }
            public string result_type { get; set; }
            public List<UrbanListItem> list { get; set; }
        }
        
        private class UrbanListItem
        {
            public string definition { get; set; }
            public string permalink { get; set; }
            public string thumbs_up { get; set; }
            public string author { get; set; }
            public string word { get; set; }
            public string defid { get; set; }
            public string current_vote { get; set; }
            public string example { get; set; }
            public string thumbs_down { get; set; }
        }
    }
}