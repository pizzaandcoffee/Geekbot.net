using System.Text;
using System.Text.Json;
using Geekbot.Core.GlobalSettings;
using Geekbot.Interactions;
using Geekbot.Interactions.Request;
using Geekbot.Interactions.Response;
using Microsoft.AspNetCore.Mvc;
using Sodium;

namespace Geekbot.Web.Controllers.Interactions;

[ApiController]
public class InteractionController : ControllerBase
{
    private readonly IInteractionCommandManager _interactionCommandManager;
    private readonly byte[] _publicKeyBytes;

    public InteractionController(IGlobalSettings globalSettings, IInteractionCommandManager interactionCommandManager)
    {
        _interactionCommandManager = interactionCommandManager;
        var publicKey = globalSettings.GetKey("DiscordPublicKey");
        _publicKeyBytes = Convert.FromHexString(publicKey.AsSpan());
    }

    [HttpPost]
    [Route("/interactions")]
    public async Task<IActionResult> HandleInteraction(
        [FromHeader(Name = "X-Signature-Ed25519")]
        string signature,
        [FromHeader(Name = "X-Signature-Timestamp")]
        string timestamp
    )
    {
        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(timestamp))
        {
            return BadRequest();
        }

        Request.EnableBuffering();
        if (!(await HasValidSignature(signature, timestamp)))
        {
            return Unauthorized();
        }

        if (Request.Body.CanSeek) Request.Body.Seek(0, SeekOrigin.Begin);
        var interaction = await JsonSerializer.DeserializeAsync<Interaction>(Request.Body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }).ConfigureAwait(false);
        if (interaction is null) throw new JsonException("Failed to deserialize JSON body");

        return (interaction.Type, interaction.Version) switch
        {
            (InteractionType.Ping, 1) => Ping(),
            (InteractionType.ApplicationCommand, 1) => await ApplicationCommand(interaction),
            (InteractionType.MessageComponent, 1) => MessageComponent(interaction),
            _ => StatusCode(501)
        };
    }

    private IActionResult Ping()
    {
        var response = new InteractionResponse()
        {
            Type = InteractionResponseType.Pong,
        };
        return Ok(response);
    }

    private async Task<IActionResult> ApplicationCommand(Interaction interaction)
    {
        var result = await _interactionCommandManager.RunCommand(interaction);

        if (result == null)
        {
            return StatusCode(501);
        }

        return Ok(result);
    }

    private IActionResult MessageComponent(Interaction interaction)
    {
        return StatusCode(501);
    }

    private async Task<bool> HasValidSignature(string signature, string timestamp)
    {
        var timestampBytes = Encoding.Default.GetBytes(timestamp);
        var signatureBytes = Convert.FromHexString(signature.AsSpan());

        var memoryStream = new MemoryStream();
        await Request.Body.CopyToAsync(memoryStream).ConfigureAwait(false);
        var body = memoryStream.ToArray();

        var timestampLength = timestampBytes.Length;
        Array.Resize(ref timestampBytes, timestampLength + body.Length);
        Array.Copy(body, 0, timestampBytes, timestampLength, body.Length);

        return PublicKeyAuth.VerifyDetached(signatureBytes, timestampBytes, _publicKeyBytes);
    }
}