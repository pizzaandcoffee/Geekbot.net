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
        private readonly Random rnd;
        private readonly ICheckEmImageProvider checkEmImages;
        public CheckEm(Random RandomClient, ICheckEmImageProvider checkEmImages)
        {
            this.rnd = RandomClient;
            this.checkEmImages = checkEmImages;
        }
        [Command("checkem", RunMode = RunMode.Async), Summary("Check for dubs")]
        public async Task MuhDubs()
        {
            try
            {
                var sb = new StringBuilder();

                sb.AppendLine($"Check em {Context.User.Mention}");
                sb.AppendLine($"**{rnd.Next(100000, 99999999)}**");
                sb.AppendLine(checkEmImages.GetRandomCheckEmPic());

                await ReplyAsync(sb.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}