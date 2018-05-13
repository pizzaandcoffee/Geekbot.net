using Geekbot.net.Database.Models;

namespace Geekbot.net.Lib.GlobalSettings
{
    public interface IGlobalSettings
    {
        bool SetKey(string keyName, string value);
        string GetKey(string keyName);
        GlobalsModel GetKeyFull(string keyName);
    }
}