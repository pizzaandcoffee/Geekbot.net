using Geekbot.Core.Converters;
using Geekbot.Interactions;
using Geekbot.Interactions.ApplicationCommand;
using Geekbot.Interactions.Request;
using Geekbot.Interactions.Response;

namespace Geekbot.Web.Commands;

public class Emojify : InteractionBase
{
    private struct Options
    {
        public const string Text = "text";
    }

    public override Command GetCommandInfo() => new ()
    {
        Name = "emojify",
        Description = "Transcribe text into emojis",
        Type = CommandType.ChatInput,
        Options = new List<Option>()
        {
            new ()
            {
                Name = Options.Text,
                Description = "The text to convert",
                Required = true,
                Type = OptionType.String,
            }
        }
    };
    public override Task<InteractionResponse> Exec(Interaction interaction)
    {
        var text = interaction.Data.Options.Find(o => o.Name == Options.Text);
        return Task.FromResult(SimpleResponse(EmojiConverter.TextToEmoji(text.Value.GetString())));
    }
}