using System;
using System.Text.Json.Serialization;

namespace Geekbot.Core.Logger;

public struct GeekbotLoggerObject
{
    public DateTime Timestamp { get; set; }
 
    public string Type { get; set; }
    
    public LogSource Source { get; set; }
    
    public string Message { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ExceptionDto? StackTrace { get; set; }
    
    public object Extra { get; set; }
}
