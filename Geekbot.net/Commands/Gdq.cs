using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
{
    public class Gdq : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        
        public Gdq(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("gdq", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Games)]
        [Summary("Get a quote from the GDQ donation generator.")]
        public async Task getQuote()
        {
            try
            {
                using (var client = new WebClient())
                {
                    var url = new Uri("http://taskinoz.com/gdq/api/");
                    var response = client.DownloadString(url);
                    
                    await ReplyAsync(response);
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}