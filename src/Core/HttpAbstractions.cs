using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public static async Task<TResponse> Get<TResponse>(Uri location, HttpClient httpClient = null, bool disposeClient = true)
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

            return JsonConvert.DeserializeObject<TResponse>(stringResponse);
        }
        
        public static async Task<TResponse> Post<TResponse>(Uri location, object data, HttpClient httpClient = null, bool disposeClient = true)
        {
            httpClient ??= CreateDefaultClient();
            httpClient.BaseAddress = location;

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(data, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                Encoding.UTF8,
                "application/json"
            );
            var response = await httpClient.PostAsync(location.PathAndQuery, content);
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();

            if (disposeClient)
            {
                httpClient.Dispose();
            }

            return JsonConvert.DeserializeObject<TResponse>(stringResponse);
        }
        
        public static async Task Post(Uri location, object data, HttpClient httpClient = null, bool disposeClient = true)
        {
            httpClient ??= CreateDefaultClient();
            httpClient.BaseAddress = location;

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(data, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.PostAsync(location, content);
            response.EnsureSuccessStatusCode();

            if (disposeClient)
            {
                httpClient.Dispose();
            }
        }
        
        public static async Task Patch(Uri location, object data, HttpClient httpClient = null, bool disposeClient = true)
        {
            httpClient ??= CreateDefaultClient();
            httpClient.BaseAddress = location;

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(data, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                Encoding.UTF8,
                "application/json"
            );

            var response = await httpClient.PatchAsync(location, content);
            response.EnsureSuccessStatusCode();

            if (disposeClient)
            {
                httpClient.Dispose();
            }
        }
        
        public static async Task Delete(Uri location, HttpClient httpClient = null, bool disposeClient = true)
        {
            httpClient ??= CreateDefaultClient();
            httpClient.BaseAddress = location;

            var response = await httpClient.DeleteAsync(location);
            response.EnsureSuccessStatusCode();

            if (disposeClient)
            {
                httpClient.Dispose();
            }
        }
    }
}