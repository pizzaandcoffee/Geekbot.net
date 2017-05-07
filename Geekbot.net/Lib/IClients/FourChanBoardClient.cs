using RestSharp;
using System;
using System.Collections.Generic;
using static Geekbot.net.Lib.Dtos.FourChanDto;

namespace Geekbot.net.Lib.IClients
{

    class FourChanBoardClient
    {
        private BoardList boards;
        private static FourChanBoardClient instace;

        private FourChanBoardClient()
        {
            Console.WriteLine("Fetching Boards");
            var boardClient = new RestClient("https://a.4cdn.org");
            var boardRequest = new RestRequest("boards.json", Method.GET);
            var boardResult = boardClient.Execute<BoardList>(boardRequest);
            this.boards = boardResult.Data;
        }

        public static FourChanBoardClient Boards()
        {
            if (instace == null)
            {
                instace = new FourChanBoardClient();
            }

            return instace;
        }

        public List<Board> getBoards()
        {
            return this.boards.Boards;
        }
    }
}
