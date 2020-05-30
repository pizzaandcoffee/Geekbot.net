namespace Geekbot.net.Lib.KvInMemoryStore
{
    public interface IKvInMemoryStore
    {
        public T Get<T>(string key);
        public void Set<T>(string key, T value);
        public void Remove(string key);
    }
}