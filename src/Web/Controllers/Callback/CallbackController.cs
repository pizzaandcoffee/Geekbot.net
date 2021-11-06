using System.Net.Http.Headers;
using System.Text.Json;
using Geekbot.Core.GlobalSettings;
using Microsoft.AspNetCore.Mvc;

namespace Geekbot.Web.Controllers.Callback;

[ApiController]
public class CallbackController : ControllerBase
{
    private readonly IGlobalSettings _globalSettings;

    public CallbackController(IGlobalSettings globalSettings)
    {
        _globalSettings = globalSettings;
    }

    [Route("/callback")]
    public async Task<IActionResult> DoCallback([FromQuery] string code)
    {
        var token = "";
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://discordapp.com");
            var appId = _globalSettings.GetKey("DiscordApplicationId");
            var accessToken = _globalSettings.GetKey("OAuthToken");
            var callbackUrl = _globalSettings.GetKey("OAuthCallbackUrl");

            var form = new Dictionary<string, string>
            {
                { "client_id", appId },
                { "client_secret", accessToken },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "scope", "identify email guilds" },
                { "redirect_uri", callbackUrl }
            };

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var result = await client.PostAsync("/api/oauth2/token", new FormUrlEncodedContent(form));
            result.EnsureSuccessStatusCode();

            var stringResponse = await result.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<CallbackTokenResponse>(stringResponse);
            token = responseData.AccessToken;
        }

        return new RedirectResult($"https://geekbot.pizzaandcoffee.rocks/login?token={token}", false);
    }
}