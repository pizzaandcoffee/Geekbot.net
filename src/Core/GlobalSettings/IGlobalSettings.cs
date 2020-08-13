using System.Threading.Tasks;
using Geekbot.Core.Database.Models;

namespace Geekbot.Core.GlobalSettings
{
    public interface IGlobalSettings
    {
        Task<bool> SetKey(string keyName, string value);
        string GetKey(string keyName);
        GlobalsModel GetKeyFull(string keyName);
    }
}