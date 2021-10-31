using System.Text.Json.Serialization;

namespace Geekbot.Core.Logger
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
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
        Interaction,
        Other
    }
}