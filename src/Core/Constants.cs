﻿using System.Reflection;

namespace Geekbot.Core
{
    public static class Constants
    {
        public const string Name = "Geekbot";

        public static string BotVersion()
        {
            return typeof(Constants).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        public static string LibraryVersion()
        {
            return typeof(Discord.WebSocket.DiscordSocketClient).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        public const double ApiVersion = 1;
    }
}