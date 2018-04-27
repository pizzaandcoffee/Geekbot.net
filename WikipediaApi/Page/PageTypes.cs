using System.Runtime.Serialization;

namespace WikipediaApi.Page
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