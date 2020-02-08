using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Utf8Json;

namespace Geekbot.net.Lib.Converters
{
    public class MtgManaConverter : IMtgManaConverter
    {
        private readonly Dictionary<string, string> _manaDict;

        public MtgManaConverter()
        {
            // these emotes can be found at https://discord.gg/bz8HyA7
            var mtgEmojis = File.ReadAllText(Path.GetFullPath("./Lib/Converters/MtgManaEmojis.json"));
            _manaDict = JsonSerializer.Deserialize<Dictionary<string, string>>(mtgEmojis);
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