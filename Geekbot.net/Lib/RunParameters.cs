using System;
using CommandLine;

namespace Geekbot.net.Lib
{
    public class RunParameters
    {
        /**
         * General Parameters
         */
        [Option('V', "verbose", Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('r', "reset", Default = false, HelpText = "Resets the bot")]
        public bool Reset { get; set; }

        [Option('j', "log-json", Default = false, HelpText = "Logs messages as json")]
        public bool LogJson { get; set; }

        [Option('a', "disable-api", Default = false, HelpText = "Disables the web api")]
        public bool DisableApi { get; set; }

        [Option('e', "expose-errors", Default = false, HelpText = "Shows internal errors in the chat")]
        public bool ExposeErrors { get; set; }
        
        [Option("token", Default = null, HelpText = "Set a new bot token")]
        public string Token { get; set; }
        
        /**
         * Database Stuff
         */
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
    }
}