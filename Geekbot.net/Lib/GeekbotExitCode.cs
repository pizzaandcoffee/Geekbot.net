namespace Geekbot.net.Lib
{
    public enum GeekbotExitCode : int
    {
        // General
        Clean = 0,
        InvalidArguments = 1,
        
        // Geekbot Internals
        TranslationsFailed = 201,
        
        // Dependent Services
        RedisConnectionFailed = 301,
        DatabaseConnectionFailed = 302,
        
        // Discord Related
        CouldNotLogin = 401
        
    }
}