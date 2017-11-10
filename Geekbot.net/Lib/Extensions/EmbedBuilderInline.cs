using Discord;

namespace Geekbot.net.Lib.Extensions
{
    public class GeekbotEmbedBuilder : EmbedBuilder
    {
        public EmbedBuilder AddInlineField(string name, object value)
        {
            this.AddField(new EmbedFieldBuilder().WithIsInline(true).WithName(name).WithValue(value));
            return this;
        }
    }
}
