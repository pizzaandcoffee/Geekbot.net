﻿using System;

namespace Geekbot.Bot.Commands.Utils.Quote
{
    internal class QuoteObjectDto
    {
        public ulong UserId { get; set; }
        public string Quote { get; set; }
        public DateTime Time { get; set; }
        public string Image { get; set; }
    }
}