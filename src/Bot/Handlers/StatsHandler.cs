using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Geekbot.Core.Database;
using Geekbot.Core.Database.Models;
using Geekbot.Core.Extensions;
using Geekbot.Core.Logger;
using Microsoft.EntityFrameworkCore;

namespace Geekbot.Bot.Handlers
{
    public class StatsHandler
    {
        private readonly IGeekbotLogger _logger;
        private readonly DatabaseContext _database;

        public StatsHandler(IGeekbotLogger logger, DatabaseContext database)
        {
            _logger = logger;
            _database = database;
        }
        
        public async Task UpdateStats(SocketMessage message)
        {
            try
            {
                if (message == null) return;
                if (message.Channel.Name.StartsWith('@'))
                {
                    _logger.Information(LogSource.Message, $"[DM-Channel] {message.Content}", SimpleConextConverter.ConvertSocketMessage(message, true));
                    return;
                }

                var channel = (SocketGuildChannel) message.Channel;

                var rowId = await _database.Database.ExecuteSqlRawAsync(
                    "UPDATE \"Messages\" SET \"MessageCount\" = \"MessageCount\" + 1 WHERE \"GuildId\" = {0} AND \"UserId\" = {1}",
                    channel.Guild.Id.AsLong(),
                    message.Author.Id.AsLong()
                );

                if (rowId == 0)
                {
                    await _database.Messages.AddAsync(new MessagesModel
                    {
                        UserId = message.Author.Id.AsLong(),
                        GuildId = channel.Guild.Id.AsLong(),
                        MessageCount = 1
                    });
                    await _database.SaveChangesAsync();
                }

                if (message.Author.IsBot) return;
                _logger.Information(LogSource.Message, message.Content, SimpleConextConverter.ConvertSocketMessage(message));
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Message, "Could not process message stats", e);
            }
        }
    }
}