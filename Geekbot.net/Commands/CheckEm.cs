﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.net.Lib;
using Geekbot.net.Lib.Media;
using Serilog;

namespace Geekbot.net.Commands
{
    public class CheckEm : ModuleBase
    {
        private readonly IMediaProvider checkEmImages;
        private readonly Random rnd;
        private readonly ILogger logger;
        private readonly IErrorHandler errorHandler;

        public CheckEm(Random RandomClient, IMediaProvider mediaProvider, ILogger logger, IErrorHandler errorHandler)
        {
            this.rnd = RandomClient;
            this.checkEmImages = mediaProvider;
            this.logger = logger;
            this.errorHandler = errorHandler;
        }

        [Command("checkem", RunMode = RunMode.Async)]
        [Remarks(CommandCategories.Randomness)]
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
                sb.AppendLine(checkEmImages.getCheckem());

                await ReplyAsync(sb.ToString());
            }
            catch (Exception e)
            {
                errorHandler.HandleCommandException(e, Context);
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