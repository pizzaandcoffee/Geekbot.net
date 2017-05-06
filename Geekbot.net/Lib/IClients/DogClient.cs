using RestSharp;

namespace Geekbot.net.Lib.IClients
{
    public interface IDogClient
    {
        IRestClient Client { get; set; }
    }

    public class DogClient : IDogClient
    {
        public DogClient()
        {
            Client = new RestClient("http://random.dog");
        }

        public IRestClient Client { get; set; }
    }
}