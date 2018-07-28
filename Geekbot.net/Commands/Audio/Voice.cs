using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib.Audio;
using Geekbot.net.Lib.ErrorHandling;

namespace Geekbot.net.Commands.Audio
{
    public class Voice : ModuleBase
    {
        private readonly IAudioUtils _audioUtils;
        private readonly IErrorHandler _errorHandler;

        public Voice(IErrorHandler errorHandler, IAudioUtils audioUtils)
        {
            _errorHandler = errorHandler;
            _audioUtils = audioUtils;
        }

//        [Command("join")]
        public async Task JoinChannel()
        {
            try
            {
                // Get the audio channel
                var channel = (Context.User as IGuildUser)?.VoiceChannel;
                if (channel == null)
                {
                    await Context.Channel.SendMessageAsync("You must be in a voice channel.");
                    return;
                }

                var audioClient = await channel.ConnectAsync();
                _audioUtils.StoreAudioClient(Context.Guild.Id, audioClient);
                await ReplyAsync($"Connected to {channel.Name}");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }

//        [Command("disconnect")]
        public async Task DisconnectChannel()
        {
            try
            {
                var audioClient = _audioUtils.GetAudioClient(Context.Guild.Id);
                if (audioClient == null)
                {
                    await Context.Channel.SendMessageAsync("I'm not in a voice channel at the moment");
                    return;
                }

                await audioClient.StopAsync();
                await ReplyAsync("Disconnected from channel!");
                _audioUtils.Cleanup(Context.Guild.Id);
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
                _audioUtils.Cleanup(Context.Guild.Id);
            }
        }

//        [Command("ytplay")]
        public async Task Ytplay(string url)
        {
            try
            {
                if (!url.Contains("youtube"))
                {
                    await ReplyAsync("I can only play youtube videos");
                    return;
                }
                var audioClient = _audioUtils.GetAudioClient(Context.Guild.Id);
                if (audioClient == null)
                {
                    await ReplyAsync("I'm not in a voice channel at the moment");
                    return;
                }

                var message = await Context.Channel.SendMessageAsync("Just a second, i'm still a bit slow at this");
                var ffmpeg = _audioUtils.CreateStreamFromYoutube(url, Context.Guild.Id);
                var output = ffmpeg.StandardOutput.BaseStream;
                await message.ModifyAsync(msg => msg.Content = "**Playing!** Please note that this feature is experimental");
                var discord = audioClient.CreatePCMStream(Discord.Audio.AudioApplication.Mixed);
                await output.CopyToAsync(discord);
                await discord.FlushAsync();
                _audioUtils.Cleanup(Context.Guild.Id);
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
                _audioUtils.Cleanup(Context.Guild.Id);
            }
        }
    }

}