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
        Message,
        UserRepository,
        Command,
        Api,
        Other
    }
}