using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Geekbot.Core
{
    public static class HttpAbstractions
    {
        public static HttpClient CreateDefaultClient()
        {
            var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {MediaTypeWithQualityHeaderValue.Parse("application/json")},
                }
            };
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Geekbot/v0.0.0 (+https://geekbot.pizzaandcoffee.rocks/)");

            return client;
        }

        public static async Task<T> Get<T>(Uri location, HttpClient httpClient = null, bool disposeClient = true)
        {
            httpClient ??= CreateDefaultClient();
            httpClient.BaseAddress = location;

            var response = await httpClient.GetAsync(location.PathAndQuery);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();

            if (disposeClient)
            {
                httpClient.Dispose();
            }

            return JsonConvert.DeserializeObject<T>(stringResponse);
        }
    }
}