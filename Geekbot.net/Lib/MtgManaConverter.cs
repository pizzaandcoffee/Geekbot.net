using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Geekbot.net.Lib
{
    public class MtgManaConverter : IMtgManaConverter
    {
        private Dictionary<string, string> _manaDict;

        public MtgManaConverter()
        {
            // these emotes can be found at https://discord.gg/bz8HyA7
            
            var manaDict = new Dictionary<string, string>();
            manaDict.Add("{0}", "<:mtg_0:415216130043412482>");
            manaDict.Add("{1}", "<:mtg_1:415216130253389835>");
            manaDict.Add("{2}", "<:mtg_2:415216130031091713>");
            manaDict.Add("{3}", "<:mtg_3:415216130467037194>");
            manaDict.Add("{4}", "<:mtg_4:415216130026635295>");
            manaDict.Add("{5}", "<:mtg_5:415216130492203008>");
            manaDict.Add("{6}", "<:mtg_6:415216130458779658>");
            manaDict.Add("{7}", "<:mtg_7:415216130190475265>");
            manaDict.Add("{8}", "<:mtg_8:415216130517630986>");
            manaDict.Add("{9}", "<:mtg_9:415216130500722689>");
            manaDict.Add("{10", "<:mtg_10:415216130450391051>");
            manaDict.Add("{11}", "<:mtg_11:415216130811101185>");
            manaDict.Add("{12}", "<:mtg_12:415216130525888532>");
            manaDict.Add("{13}", "<:mtg_13:415216130517631000>");
            manaDict.Add("{14}", "<:mtg_14:415216130165178370>");
            manaDict.Add("{15}", "<:mtg_15:415216130576089108>");
            manaDict.Add("{16}", "<:mtg_16:415216130358247425>");
            manaDict.Add("{17}", "<:mtg_17:415216130601517056>");
            manaDict.Add("{18}", "<:mtg_18:415216130462842891>");
            manaDict.Add("{19}", "<:mtg_19:415216130614099988>");
            manaDict.Add("{20}", "<:mtg_20:415216130656043038>");
            manaDict.Add("{W}", "<:mtg_white:415216131515744256>");
            manaDict.Add("{U}", "<:mtg_blue:415216130521694209>");
            manaDict.Add("{B}", "<:mtg_black:415216130873884683>");
            manaDict.Add("{R}", "<:mtg_red:415216131322806272>");
            manaDict.Add("{G}", "<:mtg_green:415216131180331009>");
            manaDict.Add("{S}", "<:mtg_s:415216131293446144>");
            manaDict.Add("{T}", "<:mtg_tap:415258392727257088>");
            manaDict.Add("{2/W}", "<:mtg_2w:415216130446065664>");
            manaDict.Add("{2/U}", "<:mtg_2u:415216130429550592>");
            manaDict.Add("{2/B}", "<:mtg_2b:415216130160984065>");
            manaDict.Add("{2/R}", "<:mtg_2r:415216130454716436>");
            manaDict.Add("{2/G}", "<:mtg_2g:415216130420899840>");
            manaDict.Add("{W/U}", "<:mtg_wu:415216130970484736>");
            manaDict.Add("{W/B}", "<:mtg_wb:415216131222011914>");
            manaDict.Add("{U/R}", "<:mtg_ur:415216130962096128>");
            manaDict.Add("{U/B}", "<:mtg_ub:415216130865758218>");
            manaDict.Add("{R/W}", "<:mtg_rw:415216130878210057>");
            manaDict.Add("{G/W}", "<:mtg_gw:415216130567962646>");
            manaDict.Add("{G/U}", "<:mtg_gu:415216130739666945>");
            manaDict.Add("{B/R}", "<:mtg_br:415216130580283394>");
            manaDict.Add("{B/G}", "<:mtg_bg:415216130781609994>");
            manaDict.Add("{U/P}", "<:mtg_up:415216130861432842>");
            manaDict.Add("{R/P}", "<:mtg_rp:415216130597322783>");
            manaDict.Add("{G/P}", "<:mtg_gp:415216130760769546>");
            manaDict.Add("{W/P}", "<:mtg_wp:415216131541041172>");
            manaDict.Add("{B/P}", "<:mtg_bp:415216130664169482>");
            
            _manaDict = manaDict;
        }

        public string ConvertMana(string mana)
        {
            var rgx = Regex.Matches(mana, "(\\{(.*?)\\})");
            foreach (Match manaTypes in rgx)
            {
                var m = _manaDict.FirstOrDefault(x => x.Key == manaTypes.Value).Value;
                if (!string.IsNullOrEmpty(m))
                {
                    mana = mana.Replace(manaTypes.Value, m);
                }
            }
            return mana;
        }
    }

    public interface IMtgManaConverter
    {
        string ConvertMana(string mana);
    }
}