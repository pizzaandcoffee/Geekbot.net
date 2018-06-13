using System.Threading.Tasks;
using Geekbot.net.Database.Models;

namespace Geekbot.net.Lib.GlobalSettings
{
    public interface IGlobalSettings
    {
        Task<bool> SetKey(string keyName, string value);
        string GetKey(string keyName);
        GlobalsModel GetKeyFull(string keyName);
    }
}