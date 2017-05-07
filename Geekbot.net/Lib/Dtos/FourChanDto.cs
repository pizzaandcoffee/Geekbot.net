using System.Collections.Generic;

namespace Geekbot.net.Lib.Dtos
{
    class FourChanDto
    {
        public class BoardList
        {
            public List<Board> Boards { get; set; }
        }

        public class Board
        {
            public string board { get; set; }
            public string title { get; set; }
            public string ws_board { get; set; }
            public string meta_description { get; set; }
        }
    }
}
