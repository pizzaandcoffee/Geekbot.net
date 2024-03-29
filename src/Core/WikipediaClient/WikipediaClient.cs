﻿using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Geekbot.Core.WikipediaClient.Page;

namespace Geekbot.Core.WikipediaClient
{
    public class WikipediaClient : IWikipediaClient
    {
        private readonly HttpClient _httpClient;
        public WikipediaClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<PagePreview> GetPreview(string pageName, string language = "en")
        {
            var response = await _httpClient.GetAsync($"https://{language}.wikipedia.org/api/rest_v1/page/summary/{pageName}");
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PagePreview>(stringResponse);
        }
    }
}