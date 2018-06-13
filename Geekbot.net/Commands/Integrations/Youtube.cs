using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.GlobalSettings;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace Geekbot.net.Commands.Integrations
{
    public class Youtube : ModuleBase
    {
        private readonly IGlobalSettings _globalSettings;
        private readonly IErrorHandler _errorHandler;

        public Youtube(IGlobalSettings globalSettings, IErrorHandler errorHandler)
        {
            _globalSettings = globalSettings;
            _errorHandler = errorHandler;
        }

        [Command("yt", RunMode = RunMode.Async)]
        [Summary("Search for something on youtube.")]
        public async Task Yt([Remainder] [Summary("Title")] string searchQuery)
        {
            var key = _globalSettings.GetKey("YoutubeKey");
            if (string.IsNullOrEmpty(key))
            {
                await ReplyAsync("No youtube key set, please tell my senpai to set one");
                return;
            }

            try
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    ApiKey = key,
                    ApplicationName = GetType().ToString()
                });

                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = searchQuery;
                searchListRequest.MaxResults = 2;

                var searchListResponse = await searchListRequest.ExecuteAsync();

                var result = searchListResponse.Items[0];

                await ReplyAsync(
                    $"\"{result.Snippet.Title}\" from \"{result.Snippet.ChannelTitle}\" https://youtu.be/{result.Id.VideoId}");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}