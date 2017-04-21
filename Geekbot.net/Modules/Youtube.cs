using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Geekbot.net.Modules
{
    public class Youtube : ModuleBase
    {
        [Command("yt"), Summary("Search for something on youtube.")]
        public async Task Yt([Remainder, Summary("A Song Title")] string searchQuery)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyDQoJvtNXPVwIcUbSeeDEchnA4a-q1go0E",
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = searchQuery; // Replace with your search term.
            searchListRequest.MaxResults = 50;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            var result = searchListResponse.Items[0];

            await ReplyAsync($"\"{result.Snippet.Title}\" from \"{result.Snippet.ChannelTitle}\" https://youtu.be/{result.Id.VideoId}");
        }
    }
}