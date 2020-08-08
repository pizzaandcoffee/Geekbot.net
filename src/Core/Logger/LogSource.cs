using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Geekbot.Core.Logger
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogSource
    {
        Geekbot,
        Rest,
        Gateway,
        Discord,
        Database,
        Message,
        UserRepository,
        Command,
        Api,
        Migration,
        HighscoreManager,
        Other
    }
}