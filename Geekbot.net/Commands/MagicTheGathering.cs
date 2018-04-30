using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;
using MtgApiManager.Lib.Service;

namespace Geekbot.net.Commands
{
    public class Magicthegathering : ModuleBase
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IMtgManaConverter _manaConverter;

        public Magicthegathering(IErrorHandler errorHandler, IMtgManaConverter manaConverter)
        {
            _errorHandler = errorHandler;
            _manaConverter = manaConverter;
        }

        [Command("mtg", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Games)]
        [Summary("Find a Magic The Gathering Card.")]
        public async Task GetCard([Remainder] [Summary("name")] string cardName)
        {
            try
            {
                var service = new CardService();
                var result = service.Where(x => x.Name, cardName);

                var card = result.All().Value.FirstOrDefault();
                if (card == null)
                {
                    await ReplyAsync("I couldn't find that card...");
                    return;
                }

                var eb = new EmbedBuilder();
                eb.Title = card.Name;
                eb.Description = card.Type;

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

                if (card.Legalities != null)
                    eb.AddField("Legality", string.Join(", ", card.Legalities.Select(e => e.Format)));

                await ReplyAsync("", false, eb.Build());
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        private Color GetColor(IEnumerable<string> colors)
        {
            var color = colors.FirstOrDefault();
            switch (color)
            {
                case "Black":
                    return new Color(203, 194, 191);
                case "White":
                    return new Color(255, 251, 213);
                case "Blue":
                    return new Color(170, 224, 250);
                case "Red":
                    return new Color(250, 170, 143);
                case "Green":
                    return new Color(155, 211, 174);
                default:
                    return new Color(204, 194, 212);
            }
        }
    }
}