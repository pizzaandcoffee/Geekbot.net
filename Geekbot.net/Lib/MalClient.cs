using System.Threading.Tasks;
using MyAnimeListSharp.Auth;
using MyAnimeListSharp.Core;
using MyAnimeListSharp.Facade.Async;
using Serilog;
using StackExchange.Redis;

namespace Geekbot.net.Lib
{
    public class MalClient : IMalClient
    {
        private readonly IDatabase _redis;
        private readonly IGeekbotLogger _logger;
        private ICredentialContext _credentials;
        private AnimeSearchMethodsAsync _animeSearch;
        private MangaSearchMethodsAsync _mangaSearch;
        
        public MalClient(IDatabase redis, IGeekbotLogger logger)
        {
            _redis = redis;
            _logger = logger;
            reloadClient();
        }

        public bool reloadClient()
        {
            var malCredentials = _redis.HashGetAll("malCredentials");
            if (malCredentials.Length != 0)
            {
                _credentials = new CredentialContext();
                foreach (var c in malCredentials)
                {
                    switch (c.Name)
                    {
                        case "Username":
                            _credentials.UserName = c.Value;
                            break;
                        case "Password":
                            _credentials.Password = c.Value;
                            break;
                    }
                }
                _animeSearch = new AnimeSearchMethodsAsync(_credentials);
                _mangaSearch = new MangaSearchMethodsAsync(_credentials);
                _logger.Debug("Geekbot", "Logged in to MAL");
                return true;
            }
            _logger.Debug("Geekbot", "No MAL Credentials Set!");
            return false;
            
        }

        public bool isLoggedIn()
        {
            return _credentials != null;
        }

        public async Task<AnimeEntry> getAnime(string query)
        {
            var response = await _animeSearch.SearchDeserializedAsync(query);
            return response.Entries.Count == 0 ? null : response.Entries[0];
        }
        
        public async Task<MangaEntry> getManga(string query)
        {
            var response = await _mangaSearch.SearchDeserializedAsync(query);
            return response.Entries.Count == 0 ? null : response.Entries[0];
        }
    }

    public interface IMalClient
    {
        bool reloadClient();
        bool isLoggedIn();
        Task<AnimeEntry> getAnime(string query);
        Task<MangaEntry> getManga(string query);
    }
}