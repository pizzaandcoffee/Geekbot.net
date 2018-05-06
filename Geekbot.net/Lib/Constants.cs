using System.Reflection;

namespace Geekbot.net.Lib
{
    public class Constants
    {
        public const string Name = "Geekbot";

        public static string BotVersion()
        {
            return typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        }

        public const double ApiVersion = 1;
    }
}