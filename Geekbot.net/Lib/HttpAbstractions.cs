using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Geekbot.net.Lib
{
    public class HttpAbstractions
    {
        public static async Task<T> Get<T>(Uri location, Dictionary<string, string> additionalHeaders = null)
        {
            using var client = new HttpClient
            {
                BaseAddress = location,
                DefaultRequestHeaders =
                {
                    Accept =
                    {
                        MediaTypeWithQualityHeaderValue.Parse("application/json")
                    }
                }
            };

            if (additionalHeaders != null)
            {
                foreach (var (name, val) in additionalHeaders)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(name, val);
                }
            }
            
            var response = await client.GetAsync(location.PathAndQuery);
            response.EnsureSuccessStatusCode();
            
            var stringResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringResponse);
        }
    }
}