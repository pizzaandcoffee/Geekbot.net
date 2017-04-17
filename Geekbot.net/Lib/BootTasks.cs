using System;
using System.IO;
using System.Threading.Tasks;
using Geekbot.net.Modules;

namespace Geekbot.net.Lib
{
    public class BootTasks
    {
        public static async Task CheckSettingsFile()
        {
            // ToDO: Check settings file, if invalid, reconfig it
//            Console.WriteLine(Path.GetFullPath("./settings.json"));
        }
    }
}