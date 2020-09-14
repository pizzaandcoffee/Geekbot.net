namespace Geekbot.Core
{
    public enum GeekbotExitCode
    {
        // General
        Clean = 0,
        InvalidArguments = 1,
        
        // Geekbot Internals
        TranslationsFailed = 201,
        KilledByApiCall = 210,
        
        // Dependent Services
        /* 301 not in use anymore (redis) */
        DatabaseConnectionFailed = 302,
        
        // Discord Related
        CouldNotLogin = 401
        
    }
}