using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord.WebSocket;
using Serilog;
using StackExchange.Redis;

namespace Geekbot.net.Lib
{
    public class TranslationHandler : ITranslationHandler
    {
        private readonly ILogger _logger;
        private readonly IDatabase _redis;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> _translations;
        private Dictionary<ulong, string> _serverLanguages;
        public List<string> _supportedLanguages;
        
        public TranslationHandler(IReadOnlyCollection<SocketGuild> clientGuilds, IDatabase redis, ILogger logger)
        {
            _logger = logger;
            _redis = redis;
            _logger.Information("[Geekbot] Loading Translations");
            LoadTranslations();
            CheckSupportedLanguages();
            LoadServerLanguages(clientGuilds);
        }

        private void LoadTranslations()
        {
            try
            {
                var translations = File.ReadAllText(Path.GetFullPath("./Storage/Translations.json"));
                _translations =
                    Utf8Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(
                        translations);
            }
            catch (Exception e)
            {
                _logger.Fatal(e, "Failed to load Translations");
                Environment.Exit(110);
            }
        }
        
        private void CheckSupportedLanguages()
        {
            try
            {
                _supportedLanguages = new List<string>();
                foreach (var lang in _translations.First().Value.First().Value)
                {
                    _supportedLanguages.Add(lang.Key);
                }
            }
            catch (Exception e)
            {
                _logger.Fatal(e, "Failed to load Translations");
                Environment.Exit(110);
            }
        }

        private void LoadServerLanguages(IReadOnlyCollection<SocketGuild> clientGuilds)
        {
            _serverLanguages = new Dictionary<ulong, string>();
            foreach (var guild in clientGuilds)
            {
                var language = _redis.HashGet($"{guild.Id}:Settings", "Language");
                if (string.IsNullOrEmpty(language) || !_supportedLanguages.Contains(language))
                {
                    _serverLanguages[guild.Id] = "EN";
                }
                else
                {
                    _serverLanguages[guild.Id] = language.ToString();
                }
            }
        }

        public string GetString(ulong guildId, string command, string stringName)
        {
            var translation = _translations[command][stringName][_serverLanguages[guildId]];
            if (!string.IsNullOrWhiteSpace(translation)) return translation;
            translation = _translations[command][stringName]["EN"];
            if (string.IsNullOrWhiteSpace(translation))
            {
                _logger.Warning($"No translation found for {command} - {stringName}");
            }
            return translation;
        }

        public bool SetLanguage(ulong guildId, string language)
        {
            try
            {
                if (!_supportedLanguages.Contains(language)) return false;
                _redis.HashSet($"{guildId}:Settings", new HashEntry[]{ new HashEntry("Language", language), });
                _serverLanguages[guildId] = language;
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e, "[Geekbot] Error while changing language");
                return false;
            }
        }

        public List<string> GetSupportedLanguages()
        {
            return _supportedLanguages;
        }
    }

    public interface ITranslationHandler
    {
        string GetString(ulong guildId, string command, string stringName);
        bool SetLanguage(ulong guildId, string language);
        List<string> GetSupportedLanguages();
    }
}