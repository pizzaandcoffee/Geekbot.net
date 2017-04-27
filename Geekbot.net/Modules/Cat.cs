using System;
using System.Threading.Tasks;
using Discord.Commands;
using RestSharp;

namespace Geekbot.net.Modules
{
    public class Cat : ModuleBase, AsyncReplier
    {
        private readonly ICatClient catClient;
        private readonly AsyncReplier _asyncReplier;
        private readonly Func<IRestRequest> _requestFunc;

        public class CatResponse
        {
            public string file { get; set; }
        }

        public Cat(ICatClient catClient)
        {
            this.catClient = catClient;
            _asyncReplier = this;
            _requestFunc = (() => new RestRequest("meow.php", Method.GET));
        }
//
//        public Cat(ICatClient catClient, Func<IRestRequest> requestFunc, AsyncReplier asyncReplier)
//        {
//            this.catClient = catClient;
//            _asyncReplier = asyncReplier ?? this;
//            _requestFunc = requestFunc ?? (() => new RestRequest("meow.php", Method.GET));
//        }

        [Command("cat", RunMode = RunMode.Async), Summary("Return a random image of a cat.")]
        public async Task Say()
        {
            var request = _requestFunc();
            var response = catClient.Client.Execute<CatResponse>(request);
            await _asyncReplier.ReplyAsyncInt(response.Data.file);
        }

        public async Task ReplyAsyncInt(dynamic data)
        {
            await ReplyAsync(data);
        }
    }

    public interface AsyncReplier
    {
        Task ReplyAsyncInt(dynamic data);
    }
}