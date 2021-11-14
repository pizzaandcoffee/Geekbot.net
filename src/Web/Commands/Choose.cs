using Geekbot.Interactions;
using Geekbot.Interactions.ApplicationCommand;
using Geekbot.Interactions.Request;
using Geekbot.Interactions.Response;
using Localization = Geekbot.Core.Localization;

namespace Geekbot.Web.Commands;

public class Choose : InteractionBase
{
    private struct Options
    {
        public const string InputOptions = "options";
        public const string Separator = "separator";
    }

    public override Command GetCommandInfo() => new ()
    {
        Name = "choose",
        Description = "Let the bot choose for you.",
        Type = CommandType.ChatInput,
        Options = new List<Option>()
        {
            new ()
            {
                Name = Options.InputOptions,
                Description = "The options, by default separated with a semicolon.",
                Type = OptionType.String,
                Required = true
            },
            new ()
            {
                Name = Options.Separator,
                Description = "The separator, by default a semicolon.",
                Type = OptionType.String,
                Required = false
            }
        }
    };

    public override Task<InteractionResponse> Exec(Interaction interaction)
    {
        var options = interaction.Data.Options.Find(o => o.Name == Options.InputOptions)?.Value.GetString();
        var separator = interaction.Data.Options.Find(o => o.Name == Options.Separator)?.Value.GetString() ?? ";";
        
        var choicesArray = options.Split(separator);
        var choice = new Random().Next(choicesArray.Length);
        return Task.FromResult(SimpleResponse(string.Format(Localization.Choose.Choice, choicesArray[choice].Trim())));
    }
}