using System;
using CommandLine;

namespace Geekbot.net.Lib
{
    public class RunParameters
    {
        [Option('V', "verbose", Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }

        [Option('r', "reset", Default = false, HelpText = "Resets the bot")]
        public bool Reset { get; set; }

        [Option('j', "log-json", Default = false, HelpText = "Logs messages as json")]
        public bool LogJson { get; set; }

        [Option("disable-api", Default = false, HelpText = "Disables the web api")]
        public bool DisableApi { get; set; }

        [Option('e', "expose-errors", Default = false, HelpText = "Shows internal errors in the chat")]
        public bool ExposeErrors { get; set; }
        
        [Option("token", Default = null, HelpText = "Set a new bot token")]
        public string Token { get; set; }
    }
}