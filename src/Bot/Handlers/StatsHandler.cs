using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Timers;
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
        private string _season;
        // ToDo: Clean up in 2021
        private bool _seasonsStarted;

        public StatsHandler(IGeekbotLogger logger, DatabaseContext database)
        {
            _logger = logger;
            _database = database;
            _season = GetSeason();
            _seasonsStarted = DateTime.Now.Year == 2021;
            
            var timer = new Timer()
            {
                Enabled = true,
                AutoReset = true,
                Interval = TimeSpan.FromHours(1).TotalMilliseconds
            };
            timer.Elapsed += (sender, args) =>
            {
                _season = GetSeason();
                _seasonsStarted = DateTime.Now.Year == 2021;
            };
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
                await UpdateTotalTable(message, channel);  
                if (_seasonsStarted)
                {
                    await UpdateSeasonsTable(message, channel);
                }
                

                if (message.Author.IsBot) return;
                _logger.Information(LogSource.Message, message.Content, SimpleConextConverter.ConvertSocketMessage(message));
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Message, "Could not process message stats", e);
            }
        }

        private async Task UpdateTotalTable(SocketMessage message, SocketGuildChannel channel)
        {
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
        }

        private async Task UpdateSeasonsTable(SocketMessage message, SocketGuildChannel channel)
        {
            var rowId = await _database.Database.ExecuteSqlRawAsync(
                "UPDATE \"MessagesSeasons\" SET \"MessageCount\" = \"MessageCount\" + 1 WHERE \"GuildId\" = {0} AND \"UserId\" = {1} AND \"Season\" = {2}",
                channel.Guild.Id.AsLong(),
                message.Author.Id.AsLong(),
                _season
            );

            if (rowId == 0)
            {
                await _database.MessagesSeasons.AddAsync(new MessageSeasonsModel()
                {
                    UserId = message.Author.Id.AsLong(),
                    GuildId = channel.Guild.Id.AsLong(),
                    Season = _season,
                    MessageCount = 1
                });
                await _database.SaveChangesAsync();
            }
        }

        private static string GetSeason()
        {
            var now = DateTime.Now;
            var year = (now.Year - 2000).ToString(CultureInfo.InvariantCulture);
            var quarter = Math.Ceiling(now.Month / 3.0).ToString(CultureInfo.InvariantCulture);
            return $"{year}Q{quarter}";
        }
    }
}