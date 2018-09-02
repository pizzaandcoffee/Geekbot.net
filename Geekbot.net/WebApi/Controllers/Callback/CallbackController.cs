using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Discord.WebSocket;
using Geekbot.net.Lib.GlobalSettings;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Geekbot.net.WebApi.Controllers.Callback
{
    public class CallbackController : Controller
    {
        private readonly DiscordSocketClient _client;
        private readonly IGlobalSettings _globalSettings;

        public CallbackController(DiscordSocketClient client, IGlobalSettings globalSettings)
        {
            _client = client;
            _globalSettings = globalSettings;
        }
        
        [Route("/callback")]
        public async Task<IActionResult> DoCallback([FromQuery] string code)
        {
            var token = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://discordapp.com");
                var appInfo = await _client.GetApplicationInfoAsync();
                var accessToken = _globalSettings.GetKey("OAuthToken");
                var callbackUrl = _globalSettings.GetKey("OAuthCallbackUrl");

                var form = new Dictionary<string, string>();
                form.Add("client_id", appInfo.Id.ToString());
                form.Add("client_secret", accessToken);
                form.Add("grant_type", "authorization_code");
                form.Add("code", code);
                form.Add("scope", "identify email guilds");
                form.Add("redirect_uri", callbackUrl);
                
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var result = await client.PostAsync("/api/oauth2/token", new FormUrlEncodedContent(form));
                result.EnsureSuccessStatusCode();

                var stringResponse = await result.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<CallbackTokenResponseDto>(stringResponse);
                token = responseData.access_token;
            }

            return new RedirectResult($"https://geekbot.pizzaandcoffee.rocks/login?token={token}", false);
        }
    }
}