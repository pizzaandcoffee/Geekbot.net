using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using RestSharp;

namespace Geekbot.net.Modules
{
    public interface ICatClient
    {
        IRestClient Client { get; set; }
    }

    public class CatClient : ICatClient
    {
        //Manage a restClient
        public CatClient()
        {
            Client = new RestClient("http://random.cat");
        }

        public IRestClient Client { get; set; }
    }
}