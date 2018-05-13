using System.Linq;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;

namespace Geekbot.net.Lib.GlobalSettings
{
    public class GlobalSettings : IGlobalSettings
    {
        private readonly DatabaseContext _database;
        
        public GlobalSettings(DatabaseContext database)
        {
            _database = database;
        }

        public bool SetKey(string keyName, string value)
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
                    _database.SaveChanges();
                    return true;
                }

                key.Value = value;
                _database.Globals.Update(key);
                _database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetKey(string keyName)
        {
            var key = _database.Globals.FirstOrDefault(k => k.Name.Equals(keyName));
            return key?.Value ?? string.Empty;
        }

        public GlobalsModel GetKeyFull(string keyName)
        {
            var key = _database.Globals.FirstOrDefault(k => k.Name.Equals(keyName));
            return key;
        }
    }
}