using System.Net.Http;
using System.Threading.Tasks;
using Geekbot.net.Lib.WikipediaClient.Page;
using Newtonsoft.Json;

namespace Geekbot.net.Lib.WikipediaClient
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
            return JsonConvert.DeserializeObject<PagePreview>(stringResponse);
        }
    }
}