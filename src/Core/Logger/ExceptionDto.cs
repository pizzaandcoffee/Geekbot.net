using System;

namespace Geekbot.Core.Logger;

public struct ExceptionDto
{
    public string Message { get; init; }
    
    public string InnerException { get; init; }
    
    public string Source { get; init; }

    public ExceptionDto(Exception exception)
    {
        Message = exception.Message;
        InnerException = string.IsNullOrEmpty(exception?.InnerException?.ToString()) ? exception?.StackTrace : exception?.InnerException?.ToString();
        Source = exception.Source;
    }
};