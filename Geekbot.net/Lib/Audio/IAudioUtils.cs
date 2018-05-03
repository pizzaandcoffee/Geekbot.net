using System.Diagnostics;
using Discord.Audio;

namespace Geekbot.net.Lib.Audio
{
    public interface IAudioUtils
    {
        IAudioClient GetAudioClient(ulong guildId);
        void StoreAudioClient(ulong guildId, IAudioClient client);
        Process CreateStreamFromFile(string path);
        Process CreateStreamFromYoutube(string url, ulong guildId);
        void Cleanup(ulong guildId);

    }
}