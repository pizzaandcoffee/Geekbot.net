using System.Collections.Generic;
using Discord.Audio;

namespace Geekbot.net.Lib
{
    public class AudioUtils : IAudioUtils
    {
        private Dictionary<ulong, IAudioClient> _audioClients;

        public AudioUtils()
        {
            _audioClients = new Dictionary<ulong, IAudioClient>();
        }

        public IAudioClient GetAudioClient(ulong guildId)
        {
            return _audioClients[guildId];
        }

        public void StoreAudioClient(ulong guildId, IAudioClient client)
        {
            _audioClients[guildId] = client;
        }
    }

    public interface IAudioUtils
    {
        IAudioClient GetAudioClient(ulong guildId);
        void StoreAudioClient(ulong guildId, IAudioClient client);
    }
}