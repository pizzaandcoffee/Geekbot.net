using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Geekbot.net.Lib.Logger
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogSource
    {
        Geekbot,
        Rest,
        Gateway,
        Discord,
        Redis,
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