using Discord;

namespace Geekbot.net.Lib.Extensions
{
    public static class EmbedBuilderExtensions
    {
        public static EmbedBuilder AddInlineField(this EmbedBuilder builder, string name, object value)
        {
            return builder.AddField(new EmbedFieldBuilder().WithIsInline(true).WithName(name).WithValue(value));
        }
    }
}