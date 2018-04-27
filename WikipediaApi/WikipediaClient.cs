using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WikipediaApi.Page;

namespace WikipediaApi
{
    public class WikipediaClient : IWikipediaClient
    {
        private readonly HttpClient _httpClient;
        public WikipediaClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://en.wikipedia.org")
            };
        }

        public async Task<PagePreview> GetPreview(string pageName)
        {
            var response = await _httpClient.GetAsync($"/api/rest_v1/page/summary/{pageName}");
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<PagePreview>(stringResponse);
        }
    }
}