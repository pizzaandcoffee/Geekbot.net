using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.Media;

namespace Geekbot.net.Commands.Randomness
{
    public class CheckEm : ModuleBase
    {
        private readonly IMediaProvider _checkEmImages;
        private readonly IErrorHandler _errorHandler;

        public CheckEm(IMediaProvider mediaProvider, IErrorHandler errorHandler)
        {
            _checkEmImages = mediaProvider;
            _errorHandler = errorHandler;
        }

        [Command("checkem", RunMode = RunMode.Async)]
        [Summary("Check for dubs")]
        public async Task MuhDubs()
        {
            try
            {
                var number = new Random().Next(10000000, 99999999);
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
                sb.AppendLine(_checkEmImages.GetCheckem());

                await ReplyAsync(sb.ToString());
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
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