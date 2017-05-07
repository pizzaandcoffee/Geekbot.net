using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using static Geekbot.net.Lib.Dtos.FourChanDto;
using Geekbot.net.Lib.IClients;

namespace Geekbot.net.Modules
{
    public class FourChan : ModuleBase
    {
        [Command("4chan", RunMode = RunMode.Async), Summary("Get Something from 4chan")]
        public async Task Chan([Summary("The someone")] string boardParam)
        {
            try
            {
                var boards = FourChanBoardClient.Boards();
                var board = new Board();
                foreach (var b in boards.getBoards())
                {
                    if (b.board.Equals(boardParam))
                    {
                        board = b;
                        break;
                    }
                }
                if (board.board == boardParam)
                {
                    await ReplyAsync($"{board.title} - {board.meta_description}");
                } else
                {
                    await ReplyAsync("Sorry, that board does not exist...");
                }                
            }
            catch (Exception e)
            {
                await ReplyAsync(e.Message);
            }
        }
    }
}

// var boards = new List<string>["a", "b", "c", "d", "e", "f", "g", "gif", "h", "hr", "k", "m", "o", "p", "r", "s", "t", "u", "v", "vg", "vr", "w", "wg", "i", "ic", "r9k", "s4s", "cm", "hm", "lgbt", "y", "3", "aco", "adv", "an", "asp", "biz", "cgl", "ck", "co", "diy", "fa", "fit", "gd", "hc", "his", "int", "jp", "lit", "mlp", "mu", "n", "news", "out", "po", "pol", "qst", "sci", "soc" / sp / tg / toy / trv / tv / vp / wsg / wsr /];
