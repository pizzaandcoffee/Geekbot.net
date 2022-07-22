using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Geekbot.Core.Database;
using Geekbot.Core.Database.Models;
using Geekbot.Core.Extensions;
using Geekbot.Core.Logger;
using Sentry;

namespace Geekbot.Core.ReactionListener
{
    public class ReactionListener : IReactionListener
    {
        private readonly DatabaseContext _database;

        private readonly IGeekbotLogger _logger;

        // <messageId, <reaction, roleId>
        private Dictionary<ulong, Dictionary<IEmote, ulong>> _listener;

        public ReactionListener(DatabaseContext database, IGeekbotLogger logger)
        {
            _database = database;
            _logger = logger;
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

        public async void RemoveRole(IMessageChannel channel, SocketReaction reaction)
        {
            _listener.TryGetValue(reaction.MessageId, out var registeredReactions);
            if (registeredReactions == null) return;
            if (!registeredReactions.ContainsKey(reaction.Emote)) return;
            var roleId = registeredReactions[reaction.Emote];
            var guild = (SocketGuildChannel) channel;
            
            try
            {
                var role = guild.Guild.GetRole(roleId);
                await ((IGuildUser) reaction.User.Value).RemoveRoleAsync(role);
            }
            catch (Exception error)
            {
                HandleDeletedRole(error, guild, reaction, roleId);
            }
        }

        public async void GiveRole(IMessageChannel channel, SocketReaction reaction)
        {
            _listener.TryGetValue(reaction.MessageId, out var registeredReactions);
            if (registeredReactions == null) return;
            if (!registeredReactions.ContainsKey(reaction.Emote)) return;
            var roleId = registeredReactions[reaction.Emote];
            var guild = (SocketGuildChannel) channel;
            
            try
            {
                
                var role = guild.Guild.GetRole(roleId);
                await ((IGuildUser) reaction.User.Value).AddRoleAsync(role);
            }
            catch (Exception error)
            {
                HandleDeletedRole(error, guild, reaction, roleId);
            }
        }

        private void HandleDeletedRole(Exception error, SocketGuildChannel guild, SocketReaction reaction, ulong roleId)
        {
            _logger.Warning(LogSource.Interaction, "Failed to get or assign role in reaction listener", error);
                
            if (!SentrySdk.IsEnabled) return;
            var sentryEvent = new SentryEvent(error)
            {
                Message = "Failed to get or assign role in reaction listener"
            };
            sentryEvent.SetTag("discord_server", guild.Id.ToString());
            sentryEvent.SetExtra("Message", reaction.MessageId.ToString());
            sentryEvent.SetExtra("User", roleId.ToString());

            SentrySdk.CaptureEvent(sentryEvent);
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