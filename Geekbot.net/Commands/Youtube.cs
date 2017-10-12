using System;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using StackExchange.Redis;

namespace Geekbot.net.Commands
{
    public class Youtube : ModuleBase
    {
        private readonly IDatabase redis;

        public Youtube(IDatabase redis)
        {
            this.redis = redis;
        }

        [Command("yt", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("Search for something on youtube.")]
        public async Task Yt([Remainder] [Summary("Title")] string searchQuery)
        {
            var key = redis.StringGet("youtubeKey");
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
                await ReplyAsync("Something went wrong... informing my senpai...");
                var botOwner = Context.Guild.GetUserAsync(ulong.Parse(redis.StringGet("botOwner"))).Result;
                var dm = await botOwner.GetOrCreateDMChannelAsync();
                await dm.SendMessageAsync(
                    $"Something went wrong while getting a video from youtube:\r\n```\r\n{e.Message}\r\n```");
            }
        }
    }
}