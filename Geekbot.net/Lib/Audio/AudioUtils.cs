using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Discord.Audio;

namespace Geekbot.net.Lib.Audio
{
    public class AudioUtils : IAudioUtils
    {
        private string _tempFolderPath;
        private Dictionary<ulong, IAudioClient> _audioClients;

        public AudioUtils()
        {
            _audioClients = new Dictionary<ulong, IAudioClient>();
            _tempFolderPath = Path.GetFullPath("./tmp/");
            if (Directory.Exists(_tempFolderPath))
            {
                Directory.Delete(_tempFolderPath, true);
            }
            Directory.CreateDirectory(_tempFolderPath);
        }
        
        public IAudioClient GetAudioClient(ulong guildId)
        {
            return _audioClients[guildId];
        }

        public void StoreAudioClient(ulong guildId, IAudioClient client)
        {
            _audioClients[guildId] = client;
        }
        
        public Process CreateStreamFromFile(string path)
        {
            var ffmpeg = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i {path} -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };
            return Process.Start(ffmpeg);
        }

        public Process CreateStreamFromYoutube(string url, ulong guildId)
        {
            var ytdlMediaUrl = GetYoutubeMediaUrl(url);
            DownloadMediaUrl(ytdlMediaUrl, guildId);
            return CreateStreamFromFile($"{_tempFolderPath}{guildId}");
        }

        public void Cleanup(ulong guildId)
        {
            File.Delete($"{_tempFolderPath}{guildId}");
        }

        private string GetYoutubeMediaUrl(string url)
        {
            var ytdl = new ProcessStartInfo()
            {
                FileName = "youtube-dl",
                Arguments = $"-f bestaudio -g {url}",
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            var output = Process.Start(ytdl).StandardOutput.ReadToEnd();
            if (string.IsNullOrWhiteSpace(output))
            {
                throw new Exception("Could not get Youtube Media URL");
            }
            return output;
        }

        private void DownloadMediaUrl(string url, ulong guildId)
        {
            using (var web = new WebClient())
            {
                web.DownloadFile(url, $"{_tempFolderPath}{guildId}");
            }
//            var ffmpeg = new ProcessStartInfo
//            {
//                FileName = "ffmpeg",
//                Arguments = $"-i \"{_tempFolderPath}{guildId}\" -c:a mp3 -b:a 256k {_tempFolderPath}{guildId}.mp3",
//                UseShellExecute = false,
//                RedirectStandardOutput = true,
//            };
//            Process.Start(ffmpeg).WaitForExit();
//            File.Delete($"{_tempFolderPath}{guildId}");
            return;
        }

    }
}