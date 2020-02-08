using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib.Converters;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Extensions;
using MtgApiManager.Lib.Service;

namespace Geekbot.net.Commands.Integrations
{
    public class MagicTheGathering : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IMtgManaConverter _manaConverter;

        public MagicTheGathering(IErrorHandler errorHandler, IMtgManaConverter manaConverter)
        {
            _errorHandler = errorHandler;
            _manaConverter = manaConverter;
        }

        [Command("mtg", RunMode = RunMode.Async)]
        [Summary("Find a Magic The Gathering Card.")]
        public async Task GetCard([Remainder] [Summary("card-name")] string cardName)
        {
            try
            {
                var service = new CardService();
                var result = service
                    .Where(x => x.Name, cardName)
                    // fewer cards less risk of deserialization problems, don't need more than one anyways...
                    .Where(x => x.PageSize, 1);

                var card = result.All().Value.FirstOrDefault();
                if (card == null)
                {
                    await ReplyAsync("I couldn't find that card...");
                    return;
                }

                var eb = new EmbedBuilder
                {
                    Title = card.Name,
                    Description = card.Type
                };

                if (card.Colors != null) eb.WithColor(GetColor(card.Colors));

                if (card.ImageUrl != null) eb.ImageUrl = card.ImageUrl.ToString();

                if (!string.IsNullOrEmpty(card.Text)) eb.AddField("Text", _manaConverter.ConvertMana(card.Text));

                if (!string.IsNullOrEmpty(card.Flavor)) eb.AddField("Flavor", card.Flavor);
                if (!string.IsNullOrEmpty(card.SetName)) eb.AddInlineField("Set", card.SetName);
                if (!string.IsNullOrEmpty(card.Power)) eb.AddInlineField("Power", card.Power);
                if (!string.IsNullOrEmpty(card.Loyalty)) eb.AddInlineField("Loyality", card.Loyalty);
                if (!string.IsNullOrEmpty(card.Toughness)) eb.AddInlineField("Thoughness", card.Toughness);

                if (!string.IsNullOrEmpty(card.ManaCost)) eb.AddInlineField("Cost", _manaConverter.ConvertMana(card.ManaCost));
                if (!string.IsNullOrEmpty(card.Rarity)) eb.AddInlineField("Rarity", card.Rarity);

                if (card.Legalities != null && card.Legalities.Count > 0)
                    eb.AddField("Legality", string.Join(", ", card.Legalities.Select(e => e.Format)));

                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

        private Color GetColor(IEnumerable<string> colors)
        {
            var color = colors.FirstOrDefault();
            return color switch
            {
                "Black" => new Color(203, 194, 191),
                "White" => new Color(255, 251, 213),
                "Blue" => new Color(170, 224, 250),
                "Red" => new Color(250, 170, 143),
                "Green" => new Color(155, 211, 174),
                _ => new Color(204, 194, 212)
            };
        }
    }
}