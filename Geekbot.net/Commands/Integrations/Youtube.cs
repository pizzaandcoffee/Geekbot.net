using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.ErrorHandling;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using StackExchange.Redis;

namespace Geekbot.net.Commands.Integrations
{
    public class Youtube : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IDatabase _redis;

        public Youtube(IDatabase redis, IErrorHandler errorHandler)
        {
            _redis = redis;
            _errorHandler = errorHandler;
        }

        [Command("yt", RunMode = RunMode.Async)]
        [Summary("Search for something on youtube.")]
        public async Task Yt([Remainder] [Summary("Title")] string searchQuery)
        {
            var key = _redis.StringGet("youtubeKey");
            if (key.IsNullOrEmpty)
            {
                await ReplyAsync("No youtube key set, please tell my senpai to set one");
                return;
            }

            try
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer
                {
                    ApiKey = key.ToString(),
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
                _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}