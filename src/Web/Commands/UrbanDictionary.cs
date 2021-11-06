using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Geekbot.Core.Interactions;
using Geekbot.Core.Interactions.ApplicationCommand;
using Geekbot.Core.Interactions.Embed;
using Geekbot.Core.Interactions.Request;
using Geekbot.Core.Interactions.Response;

namespace Geekbot.Web.Commands;

public class UrbanDictionary : InteractionBase
{
    private struct Options
    {
        internal const string Term = "term";
    }
    
    public override Command GetCommandInfo()
    {
        return new Command()
        {
            Name = "urban",
            Description = "Lookup something on urban dictionary",
            Type = CommandType.ChatInput,
            Options = new List<Option>
            {
                new ()
                {
                    Name = Options.Term,
                    Description = "The term to lookup",
                    Required = true,
                    Type = OptionType.String
                }
            }
        };
    }

    public override async Task<InteractionResponse> Exec(Interaction interaction)
    {
        var term = interaction.Data.Options.Find(o => o.Name == Options.Term);
        
        var eb = await Geekbot.Commands.UrbanDictionary.UrbanDictionary.Run(term.Value.GetString());
        return eb == null ? SimpleResponse("That word hasn't been defined...") : SimpleResponse(eb);
    }

    public override void OnException(Interaction interaction, Exception exception)
    {
        base.OnException(interaction, exception);
        Console.WriteLine(exception);
    }
}