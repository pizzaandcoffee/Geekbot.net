using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

        public static async Task<TResponse> Get<TResponse>(Uri location, HttpClient httpClient = null, bool disposeClient = true, int maxRetries = 3)
        {
            httpClient ??= CreateDefaultClient();
            httpClient.BaseAddress = location;

            HttpResponseMessage response;
            try
            {
                response = await Execute(() => httpClient.GetAsync(location.PathAndQuery), maxRetries);
            }
            finally
            {
                if (disposeClient)
                {
                    httpClient.Dispose();
                }
            }

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(stringResponse);
        }
        
        public static async Task<TResponse> Post<TResponse>(Uri location, object data, HttpClient httpClient = null, bool disposeClient = true, int maxRetries = 3)
        {
            httpClient ??= CreateDefaultClient();
            httpClient.BaseAddress = location;

            var content = new StringContent(
                JsonSerializer.Serialize(data, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response;
            try
            {
                response = await Execute(() => httpClient.PostAsync(location, content), maxRetries);
            }
            finally
            {
                if (disposeClient)
                {
                    httpClient.Dispose();
                }
            }

            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(stringResponse);
        }
        
        public static async Task Post(Uri location, object data, HttpClient httpClient = null, bool disposeClient = true, int maxRetries = 3)
        {
            httpClient ??= CreateDefaultClient();
            httpClient.BaseAddress = location;

            var content = new StringContent(
                JsonSerializer.Serialize(data, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                await Execute(() => httpClient.PostAsync(location, content), maxRetries);
            }
            finally
            {
                if (disposeClient)
                {
                    httpClient.Dispose();
                }
            }
        }
        
        public static async Task Patch(Uri location, object data, HttpClient httpClient = null, bool disposeClient = true, int maxRetries = 3)
        {
            httpClient ??= CreateDefaultClient();
            httpClient.BaseAddress = location;

            var content = new StringContent(
                JsonSerializer.Serialize(data, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull }),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                await Execute(() => httpClient.PatchAsync(location, content), maxRetries);
            }
            finally
            {
                if (disposeClient)
                {
                    httpClient.Dispose();
                }
            }
        }
        
        public static async Task Delete(Uri location, HttpClient httpClient = null, bool disposeClient = true, int maxRetries = 3)
        {
            httpClient ??= CreateDefaultClient();
            httpClient.BaseAddress = location;

            try
            {
                await Execute(() => httpClient.DeleteAsync(location), maxRetries);
            }
            finally
            {
                if (disposeClient)
                {
                    httpClient.Dispose();
                }
            }
        }

        private static async Task<HttpResponseMessage> Execute(Func<Task<HttpResponseMessage>> request, int maxRetries)
        {
            var attempt = 0;
            while (true)
            {
                var response = await request();
                if (!response.IsSuccessStatusCode)
                {
                    if (attempt >= maxRetries)
                    {
                        throw new HttpRequestException($"Request failed after {attempt} attempts");
                    }
                    
                    if (response.Headers.Contains("Retry-After"))
                    {
                        var retryAfter = response.Headers.GetValues("Retry-After").First();
                        if (retryAfter.Contains(':'))
                        {
                            var duration = DateTimeOffset.Parse(retryAfter).ToUniversalTime() - DateTimeOffset.Now.ToUniversalTime();
                            await Task.Delay(duration);
                        }
                        else
                        {
                            await Task.Delay(int.Parse(retryAfter) * 1000);                            
                        }
                    }
                    else if (response.StatusCode is HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(Math.Ceiling(attempt * 1.5)));
                    }
                    else
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    
                    attempt++;
                }
                else
                {
                    return response;
                }
            }
        }
    }
}