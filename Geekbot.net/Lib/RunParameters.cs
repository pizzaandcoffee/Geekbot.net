using CommandLine;

namespace Geekbot.net.Lib
{
    public class RunParameters
    {
        /************************************
         * General                          *
         ************************************/
        
        [Option('V', "verbose", Default = false, HelpText = "Logs everything.")]
        public bool Verbose { get; set; }

        [Option('j', "log-json", Default = false, HelpText = "Logger outputs json")]
        public bool LogJson { get; set; }

        [Option('a', "disable-api", Default = false, HelpText = "Disables the web api")]
        public bool DisableApi { get; set; }

        [Option('e', "expose-errors", Default = false, HelpText = "Shows internal errors in the chat")]
        public bool ExposeErrors { get; set; }
        
        [Option("token", Default = null, HelpText = "Set a new bot token")]
        public string Token { get; set; }
        
        /************************************
         * Database                         *
         ************************************/
        
        [Option("in-memory", Default = false, HelpText = "Uses the in-memory database instead of postgresql")]
        public bool InMemory { get; set; }
        
        // Postresql connection
        [Option("database", Default = "geekbot", HelpText = "Select a postgresql database")]
        public string DbDatabase { get; set; }

        [Option("db-host", Default = "localhost", HelpText = "Set a postgresql host (e.g. 127.0.0.1)")]
        public string DbHost { get; set; }
        
        [Option("db-port", Default = "5432", HelpText = "Set a postgresql host (e.g. 5432)")]
        public string DbPort { get; set; }

        [Option("db-user", Default = "geekbot", HelpText = "Set a postgresql user")]
        public string DbUser { get; set; }

        [Option("db-password", Default = "", HelpText = "Set a posgresql password")]
        public string DbPassword { get; set; }
        
        // Logging
        [Option("db-logging", Default = false, HelpText = "Enable database logging")]
        public bool DbLogging { get; set; }

        /************************************
         * Redis                            *
         ************************************/
        
        [Option("redis-host", Default = "127.0.0.1", HelpText = "Set a redis host")]
        public string RedisHost { get; set; }

        [Option("redis-port", Default = "6379", HelpText = "Set a redis port")]
        public string RedisPort { get; set; }

        [Option("redis-database", Default = "6", HelpText = "Select a redis database (1-15)")]
        public string RedisDatabase { get; set; }
        
        /************************************
         * WebApi                           *
         ************************************/
     
        [Option("api-host", Default = "localhost", HelpText = "Host on which the WebApi listens")]
        public string ApiHost { get; set; }
        
        [Option("api-port", Default = "12995", HelpText = "Port on which the WebApi listens")]
        public string ApiPort { get; set; }
    }
}