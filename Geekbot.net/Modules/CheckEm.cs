using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Modules
{
    public class CheckEm : ModuleBase
    {
        private readonly ICheckEmImageProvider checkEmImages;
        private readonly Random rnd;

        public CheckEm(Random RandomClient, ICheckEmImageProvider checkEmImages)
        {
            rnd = RandomClient;
            this.checkEmImages = checkEmImages;
        }

        [Command("checkem", RunMode = RunMode.Async)]
        [Summary("Check for dubs")]
        public async Task MuhDubs()
        {
            try
            {
                var number = rnd.Next(10000000, 99999999);
                var dubtriqua = "";

                var ns = GetIntArray(number);
                if (ns[7] == ns[6])
                {
                    dubtriqua = "DUBS";
                    if (ns[6] == ns[5])
                    {
                        dubtriqua = "TRIPS";
                        if (ns[5] == ns[4])
                            dubtriqua = "QUADS";
                    }
                }

                var sb = new StringBuilder();
                sb.AppendLine($"Check em {Context.User.Mention}");
                sb.AppendLine($"**{number}**");
                if (!string.IsNullOrEmpty(dubtriqua))
                    sb.AppendLine($":tada: {dubtriqua} :tada:");
                sb.AppendLine(checkEmImages.GetRandomCheckEmPic());

                await ReplyAsync(sb.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private int[] GetIntArray(int num)
        {
            var listOfInts = new List<int>();
            while (num > 0)
            {
                listOfInts.Add(num % 10);
                num = num / 10;
            }
            listOfInts.Reverse();
            return listOfInts.ToArray();
        }
    }
}