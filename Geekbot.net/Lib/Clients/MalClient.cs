using System.Threading.Tasks;
using Geekbot.net.Lib.GlobalSettings;
using Geekbot.net.Lib.Logger;
using MyAnimeListSharp.Auth;
using MyAnimeListSharp.Core;
using MyAnimeListSharp.Facade.Async;

namespace Geekbot.net.Lib.Clients
{
    public class MalClient : IMalClient
    {
        private readonly IGlobalSettings _globalSettings;
        private readonly IGeekbotLogger _logger;
        private ICredentialContext _credentials;
        private AnimeSearchMethodsAsync _animeSearch;
        private MangaSearchMethodsAsync _mangaSearch;
        
        public MalClient(IGlobalSettings globalSettings, IGeekbotLogger logger)
        {
            _globalSettings = globalSettings;
            _logger = logger;
            ReloadClient();
        }

        public bool ReloadClient()
        {
            var malCredentials = _globalSettings.GetKey("MalCredentials");
            if (!string.IsNullOrEmpty(malCredentials))
            {
                var credSplit = malCredentials.Split('|');
                _credentials = new CredentialContext()
                {
                    UserName = credSplit[0],
                    Password = credSplit[1]
                };
                _animeSearch = new AnimeSearchMethodsAsync(_credentials);
                _mangaSearch = new MangaSearchMethodsAsync(_credentials);
                _logger.Debug(LogSource.Geekbot, "Logged in to MAL");
                return true;
            }
            _logger.Debug(LogSource.Geekbot, "No MAL Credentials Set!");
            return false;
            
        }

        public bool IsLoggedIn()
        {
            return _credentials != null;
        }

        public async Task<AnimeEntry> GetAnime(string query)
        {
            var response = await _animeSearch.SearchDeserializedAsync(query);
            return response.Entries.Count == 0 ? null : response.Entries[0];
        }
        
        public async Task<MangaEntry> GetManga(string query)
        {
            var response = await _mangaSearch.SearchDeserializedAsync(query);
            return response.Entries.Count == 0 ? null : response.Entries[0];
        }
    }
}