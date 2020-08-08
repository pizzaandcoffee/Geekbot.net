using System.Threading.Tasks;
using Geekbot.net.Lib.WikipediaClient.Page;

namespace Geekbot.net.Lib.WikipediaClient
{
    public interface IWikipediaClient
    {
        Task<PagePreview> GetPreview(string pageName, string language = "en");
    }
}