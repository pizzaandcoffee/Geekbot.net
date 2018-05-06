using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib.Logger;
using StackExchange.Redis;
using Utf8Json;

namespace Geekbot.net.Lib.Localization
{
    public class TranslationHandler : ITranslationHandler
    {
        private readonly IGeekbotLogger _logger;
        private readonly IDatabase _redis;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> _translations;
        private Dictionary<ulong, string> _serverLanguages;
        private List<string> _supportedLanguages;
        
        public TranslationHandler(IReadOnlyCollection<SocketGuild> clientGuilds, IDatabase redis, IGeekbotLogger logger)
        {
            _logger = logger;
            _redis = redis;
            _logger.Information(LogSource.Geekbot, "Loading Translations");
            LoadTranslations();
            LoadServerLanguages(clientGuilds);
        }

        private void LoadTranslations()
        {
            try
            {
                var translationFile = File.ReadAllText(Path.GetFullPath("./Lib/Localization/Translations.json"));
                var rawTranslations = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(translationFile);
                var sortedPerLanguage = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
                foreach (var command in rawTranslations)
                {
                    foreach (var str in command.Value)
                    {
                        foreach (var lang in str.Value)
                        {
                            if (!sortedPerLanguage.ContainsKey(lang.Key))
                            {
                                var commandDict = new Dictionary<string, Dictionary<string, string>>();
                                var strDict = new Dictionary<string, string>();
                                strDict.Add(str.Key, lang.Value);
                                commandDict.Add(command.Key, strDict);
                                sortedPerLanguage.Add(lang.Key, commandDict);
                            }
                            if (!sortedPerLanguage[lang.Key].ContainsKey(command.Key))
                            {
                                var strDict = new Dictionary<string, string>();
                                strDict.Add(str.Key, lang.Value);
                                sortedPerLanguage[lang.Key].Add(command.Key, strDict);
                            }
                            if (!sortedPerLanguage[lang.Key][command.Key].ContainsKey(str.Key))
                            {
                                sortedPerLanguage[lang.Key][command.Key].Add(str.Key, lang.Value);
                            }
                        }
                    }
                }
                _translations = sortedPerLanguage;

                _supportedLanguages = new List<string>();
                foreach (var lang in sortedPerLanguage)
                {
                    _supportedLanguages.Add(lang.Key);
                }
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Failed to load Translations", e);
                Environment.Exit(GeekbotExitCode.TranslationsFailed.GetHashCode());
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
            var translation = _translations[_serverLanguages[guildId]][command][stringName];
            if (!string.IsNullOrWhiteSpace(translation)) return translation;
            translation = _translations[command][stringName]["EN"];
            if (string.IsNullOrWhiteSpace(translation))
            {
                _logger.Warning(LogSource.Geekbot, $"No translation found for {command} - {stringName}");
            }
            return translation;
        }

        public Dictionary<string, string> GetDict(ICommandContext context)
        {
            try
            {
                var command = context.Message.Content.Split(' ').First().TrimStart('!').ToLower();
                return _translations[_serverLanguages[context.Guild.Id]][command];
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "No translations for command found", e);
                return new Dictionary<string, string>();
            }
        }
        
        public Dictionary<string, string> GetDict(ICommandContext context, string command)
        {
            try
            {
                return _translations[_serverLanguages[context.Guild.Id]][command];
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "No translations for command found", e);
                return new Dictionary<string, string>();    
            }
        }

        public bool SetLanguage(ulong guildId, string language)
        {
            try
            {
                if (!_supportedLanguages.Contains(language)) return false;
                _redis.HashSet($"{guildId}:Settings", new[]{ new HashEntry("Language", language) });
                _serverLanguages[guildId] = language;
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Error while changing language", e);
                return false;
            }
        }

        public List<string> GetSupportedLanguages()
        {
            return _supportedLanguages;
        }
    }
}