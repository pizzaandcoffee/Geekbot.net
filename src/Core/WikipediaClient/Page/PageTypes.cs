using System.Runtime.Serialization;

namespace Geekbot.Core.WikipediaClient.Page
{
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