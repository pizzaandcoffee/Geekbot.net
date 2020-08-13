using System.Threading.Tasks;
using Geekbot.Core.WikipediaClient.Page;

namespace Geekbot.Core.WikipediaClient
{
    public interface IWikipediaClient
    {
        Task<PagePreview> GetPreview(string pageName, string language = "en");
    }
}