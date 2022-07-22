using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Discord;

namespace Geekbot.Core.Polyfills
{
    public class UserPolyfillDto : IUser
    {
        public ulong Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Mention { get; set; }
        public IActivity Activity { get; }
        public UserStatus Status { get; set; }
        IReadOnlyCollection<ClientType> IPresence.ActiveClients => ActiveClients;

        IReadOnlyCollection<IActivity> IPresence.Activities => Activities;

        public IImmutableSet<ClientType> ActiveClients { get; }
        public IImmutableList<IActivity> Activities { get; }
        public Task<IDMChannel> CreateDMChannelAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }

        public string AvatarId { get; set; }
        public string AvatarUrl { get; set; }
        public string Discriminator { get; set; }
        public ushort DiscriminatorValue { get; set; }
        public bool IsBot { get; set; }
        public bool IsWebhook { get; set; }
        public string Username { get; set; }
        public UserProperties? PublicFlags { get; }

        public string GetAvatarUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
        {
            return AvatarUrl ?? "https://discordapp.com/assets/6debd47ed13483642cf09e832ed0bc1b.png";
        }

        public string GetDefaultAvatarUrl()
        {
            throw new NotImplementedException();
        }

        public Task<IDMChannel> GetOrCreateDMChannelAsync(RequestOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}