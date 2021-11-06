using Geekbot.Core;

namespace Geekbot.Commands.UrbanDictionary;

public class UrbanDictionary
{
    public static async Task<UrbanDictionaryListItem?> Run(string term)
    {
        var definitions = await HttpAbstractions.Get<UrbanDictionaryResponse>(new Uri($"https://api.urbandictionary.com/v0/define?term={term}"));
        
        if (definitions.List.Count == 0)
        {
            return null;
        }
        
        var definition = definitions.List.First(e => !string.IsNullOrWhiteSpace(e.Example));

        return definition;
    }
}