using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Geekbot.net.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Geekbot.net.Lib
{
    public class BootTasks
    {
        public static async Task CheckSettingsFile()
        {
            // ToDO: Check settings file, if invalid, reconfig it
//            Console.WriteLine(Path.GetFullPath("./settings.json"));
        }

        public static void ParseOldDatabase(string path)
        {
            Console.WriteLine("Starting Database Conversion...");
            path = Path.GetFullPath(path);
            Console.WriteLine($"Old db location: {path}");
            var redis = new RedisClient().Client;
            Console.WriteLine("Connected to Redis...");
            var allfiles = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
            foreach ( var file in allfiles)
            {
                var info = new FileInfo(file);
                if (info.Name.StartsWith("-")) continue;
                Console.WriteLine(info.FullName);
                dynamic json = JObject.Parse(File.ReadAllText(file));
                var key = info.Name.Substring(0, info.Name.Length - 5) + "-messages";
                Console.WriteLine($"{key} - {json.messages}");
                redis.StringSet(key, json.messages.ToString());
            }
        }
    }
}