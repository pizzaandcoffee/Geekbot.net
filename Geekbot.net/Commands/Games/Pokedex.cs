using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using PokeAPI;

namespace Geekbot.net.Commands.Games
{
    public class Pokedex : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;

        public Pokedex(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [Command("pokedex", RunMode = RunMode.Async)]
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

                var embed = await PokemonEmbedBuilder(pokemon);
                await ReplyAsync("", false, embed.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private async Task<EmbedBuilder> PokemonEmbedBuilder(Pokemon pokemon)
        {
            var eb = new EmbedBuilder();
            var species = await DataFetcher.GetApiObject<PokemonSpecies>(pokemon.ID);
            eb.Title = $"#{pokemon.ID} {ToUpper(pokemon.Name)}";
            eb.Description = species.FlavorTexts[1].FlavorText;
            eb.ThumbnailUrl = pokemon.Sprites.FrontMale ?? pokemon.Sprites.FrontFemale;
            eb.AddInlineField(GetSingularOrPlural(pokemon.Types.Length, "Type"),
                string.Join(", ", pokemon.Types.Select(t => ToUpper(t.Type.Name))));
            eb.AddInlineField(GetSingularOrPlural(pokemon.Abilities.Length, "Ability"),
                string.Join(", ", pokemon.Abilities.Select(t => ToUpper(t.Ability.Name))));
            eb.AddInlineField("Height", pokemon.Height);
            eb.AddInlineField("Weight", pokemon.Mass);
            return eb;
        }

        private string GetSingularOrPlural(int lenght, string word)
        {
            if (lenght == 1) return word;
            return word.EndsWith("y") ? $"{word.Remove(word.Length - 1)}ies" : $"{word}s";
        }

        private string ToUpper(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}