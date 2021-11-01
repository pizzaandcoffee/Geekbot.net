using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Geekbot.Core.WikipediaClient.Page
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PageTypes
    {
        [EnumMember(Value = "standard")] 
        Standard,
        
        [EnumMember(Value = "disambiguation")] 
        Disambiguation,
        
        [EnumMember(Value = "mainpage")] 
        MainPage,
        
        [EnumMember(Value = "no-extract")] 
        NoExtract
    }
}