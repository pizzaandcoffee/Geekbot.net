using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;

namespace Geekbot.net.Lib.GlobalSettings
{
    public class GlobalSettings : IGlobalSettings
    {
        private readonly DatabaseContext _database;
        private readonly Dictionary<string, string> _cache;
        
        public GlobalSettings(DatabaseContext database)
        {
            _database = database;
            _cache = new Dictionary<string, string>();
        }

        public async Task<bool> SetKey(string keyName, string value)
        {
            try
            {
                var key = GetKeyFull(keyName);
                if (key == null)
                {
                    _database.Globals.Add(new GlobalsModel()
                    {
                        Name = keyName,
                        Value = value
                    });
                    await _database.SaveChangesAsync();
                    return true;
                }
                key.Value = value;
                _database.Globals.Update(key);
                _cache[keyName] = value;
                await _database.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetKey(string keyName)
        {
            var keyValue = "";
            if (string.IsNullOrEmpty(_cache.GetValueOrDefault(keyName)))
            {
                keyValue = _database.Globals.FirstOrDefault(k => k.Name.Equals(keyName))?.Value ?? string.Empty;
                _cache[keyName] = keyValue;
            }
            else
            {
                keyValue = _cache[keyName];
            }
            return keyValue ;
        }

        public GlobalsModel GetKeyFull(string keyName)
        {
            var key = _database.Globals.FirstOrDefault(k => k.Name.Equals(keyName));
            return key;
        }
    }
}