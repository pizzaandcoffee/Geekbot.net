﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Geekbot.Core.Converters
{
    public class MtgManaConverter : IMtgManaConverter
    {
        private readonly Dictionary<string, string> _manaDict;

        public MtgManaConverter()
        {
            // these emotes can be found at https://discord.gg/bz8HyA7
            _manaDict = new Dictionary<string, string>
            {
                {"{0}", "<:mtg_0:415216130043412482>"},
                {"{1}", "<:mtg_1:415216130253389835>"},
                {"{2}", "<:mtg_2:415216130031091713>"},
                {"{3}", "<:mtg_3:415216130467037194>"},
                {"{4}", "<:mtg_4:415216130026635295>"},
                {"{5}", "<:mtg_5:415216130492203008>"},
                {"{6}", "<:mtg_6:415216130458779658>"},
                {"{7}", "<:mtg_7:415216130190475265>"},
                {"{8}", "<:mtg_8:415216130517630986>"},
                {"{9}", "<:mtg_9:415216130500722689>"},
                {"{10", "<:mtg_10:415216130450391051>"},
                {"{11}", "<:mtg_11:415216130811101185>"},
                {"{12}", "<:mtg_12:415216130525888532>"},
                {"{13}", "<:mtg_13:415216130517631000>"},
                {"{14}", "<:mtg_14:415216130165178370>"},
                {"{15}", "<:mtg_15:415216130576089108>"},
                {"{16}", "<:mtg_16:415216130358247425>"},
                {"{17}", "<:mtg_17:415216130601517056>"},
                {"{18}", "<:mtg_18:415216130462842891>"},
                {"{19}", "<:mtg_19:415216130614099988>"},
                {"{20}", "<:mtg_20:415216130656043038>"},
                {"{W}", "<:mtg_white:415216131515744256>"},
                {"{U}", "<:mtg_blue:415216130521694209>"},
                {"{B}", "<:mtg_black:415216130873884683>"},
                {"{R}", "<:mtg_red:415216131322806272>"},
                {"{G}", "<:mtg_green:415216131180331009>"},
                {"{S}", "<:mtg_s:415216131293446144>"},
                {"{T}", "<:mtg_tap:415258392727257088>"},
                {"{C}", "<:mtg_colorless:415216130706374666>"},
                {"{2/W}", "<:mtg_2w:415216130446065664>"},
                {"{2/U}", "<:mtg_2u:415216130429550592>"},
                {"{2/B}", "<:mtg_2b:415216130160984065>"},
                {"{2/R}", "<:mtg_2r:415216130454716436>"},
                {"{2/G}", "<:mtg_2g:415216130420899840>"},
                {"{W/U}", "<:mtg_wu:415216130970484736>"},
                {"{W/B}", "<:mtg_wb:415216131222011914>"},
                {"{U/R}", "<:mtg_ur:415216130962096128>"},
                {"{U/B}", "<:mtg_ub:415216130865758218>"},
                {"{R/W}", "<:mtg_rw:415216130878210057>"},
                {"{G/W}", "<:mtg_gw:415216130567962646>"},
                {"{G/U}", "<:mtg_gu:415216130739666945>"},
                {"{B/R}", "<:mtg_br:415216130580283394>"},
                {"{B/G}", "<:mtg_bg:415216130781609994>"},
                {"{U/P}", "<:mtg_up:415216130861432842>"},
                {"{R/P}", "<:mtg_rp:415216130597322783>"},
                {"{G/P}", "<:mtg_gp:415216130760769546>"},
                {"{W/P}", "<:mtg_wp:415216131541041172>"},
                {"{B/P}", "<:mtg_bp:415216130664169482>"}
            };
        }

        public string ConvertMana(string mana)
        {
            var rgx = Regex.Matches(mana, @"(\{(.*?)\})");
            foreach (Match manaTypes in rgx)
            {
                var m = _manaDict.GetValueOrDefault(manaTypes.Value);
                if (!string.IsNullOrEmpty(m))
                {
                    mana = mana.Replace(manaTypes.Value, m);
                }
            }

            return mana;
        }
    }
}