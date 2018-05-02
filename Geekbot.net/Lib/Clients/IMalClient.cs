using System.Threading.Tasks;
using MyAnimeListSharp.Core;

namespace Geekbot.net.Lib.Clients
{
    public interface IMalClient
    {
        bool IsLoggedIn();
        Task<AnimeEntry> GetAnime(string query);
        Task<MangaEntry> GetManga(string query);
    }
}