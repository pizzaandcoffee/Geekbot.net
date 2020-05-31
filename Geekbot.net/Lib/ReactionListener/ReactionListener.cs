using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.Extensions;

namespace Geekbot.net.Lib.ReactionListener
{
    public class ReactionListener : IReactionListener
    {
        private readonly DatabaseContext _database;
        // <messageId, <reaction, roleId>
        private Dictionary<ulong, Dictionary<IEmote, ulong>> _listener;

        public ReactionListener(DatabaseContext database)
        {
            _database = database;
            LoadListeners();
        }

        private void LoadListeners()
        {
            _listener = new Dictionary<ulong, Dictionary<IEmote, ulong>>();
            foreach (var row in _database.ReactionListeners)
            {
                var messageId = row.MessageId.AsUlong();
                if (!_listener.ContainsKey(messageId))
                {
                    _listener.Add(messageId, new Dictionary<IEmote, ulong>());
                }
                
                _listener[messageId].Add(ConvertStringToEmote(row.Reaction), row.RoleId.AsUlong());
            }
        }

        public bool IsListener(ulong id)
        {
            return _listener.ContainsKey(id);
        }

        public async Task AddRoleToListener(ulong messageId, ulong guildId, string emoji, IRole role)
        {
            var emote = ConvertStringToEmote(emoji);

            await _database.ReactionListeners.AddAsync(new ReactionListenerModel()
            {
                GuildId = guildId.AsLong(),
                MessageId = messageId.AsLong(),
                RoleId = role.Id.AsLong(),
                Reaction = emoji
            });
            await _database.SaveChangesAsync();

            if (!_listener.ContainsKey(messageId))
            {
                _listener.Add(messageId, new Dictionary<IEmote, ulong>());
            }
            _listener[messageId].Add(emote, role.Id);
        }

        public async void RemoveRole(ISocketMessageChannel channel, SocketReaction reaction)
        {
            var roleId = _listener[reaction.MessageId][reaction.Emote];
            var guild = (SocketGuildChannel) channel;
            var role = guild.Guild.GetRole(roleId);
            await ((IGuildUser) reaction.User.Value).RemoveRoleAsync(role);
        }

        public async void GiveRole(ISocketMessageChannel channel, SocketReaction reaction)
        {
            var roleId = _listener[reaction.MessageId][reaction.Emote];
            var guild = (SocketGuildChannel) channel;
            var role = guild.Guild.GetRole(roleId);
            await ((IGuildUser) reaction.User.Value).AddRoleAsync(role);
        }

        public IEmote ConvertStringToEmote(string emoji)
        {
            if (!emoji.StartsWith('<'))
            {
                return new Emoji(emoji);
            }
            return Emote.Parse(emoji);
        }
    }
}