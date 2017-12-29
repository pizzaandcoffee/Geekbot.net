using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Geekbot.net.Lib;

namespace Geekbot.net.Commands
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

        [Command("join")]
        public async Task JoinChannel()
        {
            try
            {
                // Get the audio channel
                var channel = (Context.User as IGuildUser)?.VoiceChannel;
                if (channel == null)
                {
                    await Context.Channel.SendMessageAsync(
                        "User must be in a voice channel, or a voice channel must be passed as an argument.");
                    return;
                }

                // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
                var audioClient = await channel.ConnectAsync();
                _audioUtils.StoreAudioClient(Context.Guild.Id, audioClient);
                await ReplyAsync($"Connected to {channel.Name}");
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

        [Command("disconnect")]
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
            }
            catch (Exception e)
            {
                _errorHandler.HandleCommandException(e, Context);
            }
        }

//        [Command("play")]
//        public async Task play(IVoiceChannel channel = null)
//        {
//            try
//            {
//                var audioClient = _audioUtils.GetAudioClient(Context.Guild.Id);
//                if (audioClient == null)
//                {
//                    await Context.Channel.SendMessageAsync("I'm not in a voice channel at the moment");
//                    return;
//                }
//
//            }
//            catch (Exception e)
//            {
//                _errorHandler.HandleCommandException(e, Context);
//            }
//        }
    }
}