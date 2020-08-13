using Discord;

namespace Geekbot.Core.Extensions
{
    public static class EmbedBuilderExtensions
    {
        public static EmbedBuilder AddInlineField(this EmbedBuilder builder, string name, object value)
        {
            return builder.AddField(new EmbedFieldBuilder().WithIsInline(true).WithName(name).WithValue(value));
        }
    }
}