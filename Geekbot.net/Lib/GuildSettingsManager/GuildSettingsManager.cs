using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geekbot.net.Database;
using Geekbot.net.Database.Models;
using Geekbot.net.Lib.Extensions;

namespace Geekbot.net.Lib.GuildSettingsManager
{
    public class GuildSettingsManager : IGuildSettingsManager
    {
        private readonly DatabaseContext _database;
        private readonly Dictionary<ulong, GuildSettingsModel> _settings;

        public GuildSettingsManager(DatabaseContext database)
        {
            _database = database;
            _settings = new Dictionary<ulong, GuildSettingsModel>();
        }

        public GuildSettingsModel GetSettings(ulong guildId, bool createIfNonExist = true)
        {
            return _settings.ContainsKey(guildId) ? _settings[guildId] : GetFromDatabase(guildId, createIfNonExist);
        }

        public async Task UpdateSettings(GuildSettingsModel settings)
        {
            _database.GuildSettings.Update(settings);
            if (_settings.ContainsKey(settings.GuildId.AsUlong()))
            {
                _settings[settings.GuildId.AsUlong()] = settings;
            }
            else
            {
                _settings.Add(settings.GuildId.AsUlong(), settings);
            }
            await _database.SaveChangesAsync();
        }

        private GuildSettingsModel GetFromDatabase(ulong guildId, bool createIfNonExist)
        {
            var settings = _database.GuildSettings.FirstOrDefault(guild => guild.GuildId.Equals(guildId.AsLong()));
            if (createIfNonExist && settings == null)
            {
                settings = CreateSettings(guildId);
            }
            
            _settings.Add(guildId, settings);
            return settings;
        }

        private GuildSettingsModel CreateSettings(ulong guildId)
        {
            _database.GuildSettings.Add(new GuildSettingsModel
            {
                GuildId = guildId.AsLong(),
                Hui = false,
                Ping = false,
                Language = "EN",
                ShowDelete = false,
                ShowLeave = false,
                WikiLang = "en"
            });
            _database.SaveChanges();
            return _database.GuildSettings.FirstOrDefault(g => g.GuildId.Equals(guildId.AsLong()));
        }
    }
}