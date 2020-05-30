using System.Collections.Generic;

namespace Geekbot.net.Lib.KvInMemoryStore
{
    public class KvInInMemoryStore : IKvInMemoryStore
    {
        private readonly Dictionary<string, object> _storage = new Dictionary<string, object>();

        public T Get<T>(string key)
        {
            try
            {
                return (T) _storage[key];
            }
            catch
            {
                return default;
            }
        }

        public void Set<T>(string key, T value)
        {
            _storage.Remove(key);
            _storage.Add(key, value);
        }

        public void Remove(string key)
        {
            _storage.Remove(key);
        }
    }
}