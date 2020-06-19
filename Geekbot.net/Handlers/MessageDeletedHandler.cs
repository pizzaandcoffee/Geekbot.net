using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Geekbot.net.Database;
using Geekbot.net.Lib.Extensions;
using Geekbot.net.Lib.Logger;

namespace Geekbot.net.Handlers
{
    public class MessageDeletedHandler
    {
        private readonly DatabaseContext _database;
        private readonly IGeekbotLogger _logger;
        private readonly IDiscordClient _client;

        public MessageDeletedHandler(DatabaseContext database, IGeekbotLogger logger, IDiscordClient client)
        {
            _database = database;
            _logger = logger;
            _client = client;
        }

        public async Task HandleMessageDeleted(Cacheable<IMessage, ulong> message, ISocketMessageChannel channel)
        {
            try
            {
                var guildSocketData = ((IGuildChannel) channel).Guild;
                var guild = _database.GuildSettings.FirstOrDefault(g => g.GuildId.Equals(guildSocketData.Id.AsLong()));
                if ((guild?.ShowDelete ?? false) && guild?.ModChannel != 0)
                {
                    var modChannelSocket = (ISocketMessageChannel) await _client.GetChannelAsync(guild.ModChannel.AsUlong());
                    var sb = new StringBuilder();
                    if (message.Value != null)
                    {
                        sb.AppendLine($"The following message from {message.Value.Author.Username}#{message.Value.Author.Discriminator} was deleted in <#{channel.Id}>");
                        sb.AppendLine(message.Value.Content);
                    }
                    else
                    {
                        sb.AppendLine("Someone deleted a message, the message was not cached...");
                    }

                    await modChannelSocket.SendMessageAsync(sb.ToString());
                }
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Failed to send delete message...", e);
            }
        }
    }
}