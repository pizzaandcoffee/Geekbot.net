using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using YamlDotNet.Serialization;

namespace Tests.Lib.Localization
{
    public class Translations_test
    {
        [Fact]
        public void TranslationsYamlIsValid()
        {
            // Read the file
            var translationFile = File.ReadAllText(Path.GetFullPath("./../../../../Geekbot.net/Lib/Localization/Translations.yml"));
                
            // Deserialize
            var input = new StringReader(translationFile);
            var deserializer = new DeserializerBuilder().Build();
            var rawTranslations = deserializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(input);
            
            // These languages must be supported
            var supportedLanguages = new List<string>
            {
                "EN",
                "CHDE"
            };
            
            // Iterate every single key to make sure it's populated
            foreach (var command in rawTranslations)
            {
                foreach (var str in command.Value)
                {
                    str.Value.Select(e => e.Key).ToList().Should().BeEquivalentTo(supportedLanguages, str.Key);
                    foreach (var lang in str.Value)
                    {
                        lang.Value.Should().NotBeNullOrEmpty($"{command.Key} / {str.Key} / {lang.Key}");
                    }
                }
            }
        }
    }
}