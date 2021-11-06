using System.Drawing;
using Geekbot.Core;
using Geekbot.Interactions.Embed;

namespace Geekbot.Commands.UrbanDictionary;

public class UrbanDictionary
{
    public static async Task<Embed?> Run(string term)
    {
        var definitions = await HttpAbstractions.Get<UrbanDictionaryResponse>(new Uri($"https://api.urbandictionary.com/v0/define?term={term}"));
        
        if (definitions.List.Count == 0)
        {
            return null;
        }
        
        var definition = definitions.List.First(e => !string.IsNullOrWhiteSpace(e.Example));

        static string ShortenIfToLong(string str, int maxLength) => str.Length > maxLength ? $"{str[..(maxLength - 5)]}[...]" : str;

        var eb = new Embed();
        eb.Author = new()
        {
            Name = definition.Word,
            Url = definition.Permalink
        };
        eb.SetColor(Color.Gold);
        
        if (!string.IsNullOrEmpty(definition.Definition)) eb.Description = ShortenIfToLong(definition.Definition, 1800);
        if (!string.IsNullOrEmpty(definition.Example)) eb.AddField("Example", ShortenIfToLong(definition.Example, 1024));
        if (definition.ThumbsUp != 0) eb.AddInlineField("Upvotes", definition.ThumbsUp.ToString());
        if (definition.ThumbsDown != 0) eb.AddInlineField("Downvotes", definition.ThumbsDown.ToString());

        return eb;
    }
}