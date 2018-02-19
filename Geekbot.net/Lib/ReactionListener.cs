using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using StackExchange.Redis;

namespace Geekbot.net.Lib
{
    public class ReactionListener : IReactionListener
    {
        private readonly IDatabase _database;
        private Dictionary<string, Dictionary<IEmote, ulong>> _listener;

        public ReactionListener(IDatabase database)
        {
            _database = database;
            LoadListeners();
        }

        private Task LoadListeners()
        {
            var ids = _database.SetMembers("MessageIds");
            _listener = new Dictionary<string, Dictionary<IEmote, ulong>>();
            foreach (var id in ids)
            {
                var reactions = _database.HashGetAll($"Messages:{id}");
                var messageId = id;
                var emojiDict = new Dictionary<IEmote, ulong>();
                foreach (var r in reactions)
                {
                    IEmote emote;
                    if (!r.Name.ToString().StartsWith('<'))
                    {
                        var emo = new Emoji(r.Name);
                        emote = (IEmote) emo;
                    }
                    else
                    {
                        emote = Emote.Parse(r.Name);
                    }
                    emojiDict.Add(emote, ulong.Parse(r.Value));
                }
                _listener.Add(messageId, emojiDict);
            }

            return Task.CompletedTask;
        }

        public bool IsListener(ulong id)
        {
            return _listener.ContainsKey(id.ToString());
        }

        public Task AddRoleToListener(string messageId, IEmote emoji, IRole role)
        {
            if (_database.SetMembers("MessageIds").All(e => e.ToString() != messageId))
            {
                _database.SetAdd("MessageIds", messageId);
            }
            _database.HashSet($"Messages:{messageId}",  new[] {new HashEntry(emoji.ToString(), role.Id.ToString())});
            _database.SetAdd("MessageIds", messageId);
            if (_listener.ContainsKey(messageId))
            {
                _listener[messageId].Add(emoji, role.Id);
                return Task.CompletedTask;
            }
            var dict = new Dictionary<IEmote, ulong>();
            dict.Add(emoji, role.Id);
            _listener.Add(messageId, dict);
            return Task.CompletedTask;
        }

        public async void RemoveRole(ISocketMessageChannel channel, SocketReaction reaction)
        {
            var roleId = _listener[reaction.MessageId.ToString()][reaction.Emote];
            var guild = (SocketGuildChannel) channel;
            var role = guild.Guild.GetRole(roleId);
            await ((IGuildUser) reaction.User.Value).RemoveRoleAsync(role);
        }

        public async void GiveRole(ISocketMessageChannel channel, SocketReaction reaction)
        {
            var roleId = _listener[reaction.MessageId.ToString()][reaction.Emote];
            var guild = (SocketGuildChannel) channel;
            var role = guild.Guild.GetRole(roleId);
            await ((IGuildUser) reaction.User.Value).AddRoleAsync(role);
        }
    }
    
    public interface IReactionListener
    {
        bool IsListener(ulong id);
        Task AddRoleToListener(string messageId, IEmote emoji, IRole role);
        void RemoveRole(ISocketMessageChannel channel, SocketReaction reaction);
        void GiveRole(ISocketMessageChannel message, SocketReaction reaction);
    }
}