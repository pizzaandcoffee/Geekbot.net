using System.Threading.Tasks;
using WikipediaApi.Page;

namespace WikipediaApi
{
    public interface IWikipediaClient
    {
        Task<PagePreview> GetPreview(string pageName);
    }
}