using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Extensions;
using PokeAPI;
using Serilog;

namespace Geekbot.net.Commands
{
    public class Pokedex : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        
        public Pokedex(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("pokedex", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Helpers)]
        [Summary("A Pokedex Tool")]
        public async Task GetPokemon([Summary("pokemonName")] string pokemonName)
        {
            try
            {
                DataFetcher.ShouldCacheData = true;
                Pokemon pokemon;
                try
                {
                    pokemon = await DataFetcher.GetNamedApiObject<Pokemon>(pokemonName.ToLower());
                }
                catch
                {
                    await ReplyAsync("I couldn't find that pokemon :confused:");
                    return;
                }
                var embed = await pokemonEmbedBuilder(pokemon);
                await ReplyAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private async Task<EmbedBuilder> pokemonEmbedBuilder(Pokemon pokemon)
        {
            var eb = new GeekbotEmbedBuilder();
            var species = await DataFetcher.GetApiObject<PokemonSpecies>(pokemon.ID);
            eb.Title = $"#{pokemon.ID} {toUpper(pokemon.Name)}";
            eb.Description = species.FlavorTexts[1].FlavorText;
            eb.ThumbnailUrl = pokemon.Sprites.FrontMale ?? pokemon.Sprites.FrontFemale;
            eb.AddInlineField(getSingularOrPlural(pokemon.Types.Length, "Type"), string.Join(", ", pokemon.Types.Select(t => toUpper(t.Type.Name))));
            eb.AddInlineField(getSingularOrPlural(pokemon.Abilities.Length, "Ability"), string.Join(", ", pokemon.Abilities.Select(t => toUpper(t.Ability.Name))));
            eb.AddInlineField("Height", pokemon.Height);
            eb.AddInlineField("Weight", pokemon.Mass);
            return eb;
        }

        private string getSingularOrPlural(int lenght, string word)
        {
            if (lenght == 1)
            {
                return word;
            }
            return word.EndsWith("y") ? $"{word.Remove(word.Length-1)}ies" : $"{word}s";
        }
        
        private string toUpper(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }
        
            
    }
}