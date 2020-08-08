using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Geekbot.Core.GuildSettingsManager;
using Geekbot.Core.Logger;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Geekbot.Core.Localization
{
    public class TranslationHandler : ITranslationHandler
    {
        private readonly IGeekbotLogger _logger;
        private readonly IGuildSettingsManager _guildSettingsManager;
        private readonly Dictionary<ulong, string> _serverLanguages;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> _translations;

        public TranslationHandler(IGeekbotLogger logger, IGuildSettingsManager guildSettingsManager)
        {
            _logger = logger;
            _guildSettingsManager = guildSettingsManager;
            _logger.Information(LogSource.Geekbot, "Loading Translations");
            LoadTranslations();
            _serverLanguages = new Dictionary<ulong, string>();
        }

        private void LoadTranslations()
        {
            try
            {
                // Read the file
                var translationFile = File.ReadAllText(Path.GetFullPath("./Lib/Localization/Translations.yml"));
                
                // Deserialize
                var input = new StringReader(translationFile);
                var mergingParser = new MergingParser(new Parser(input));
                var deserializer = new DeserializerBuilder().Build();
                var rawTranslations = deserializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(mergingParser);
                
                // Sort
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
                                var strDict = new Dictionary<string, string>
                                {
                                    {str.Key, lang.Value}
                                };
                                commandDict.Add(command.Key, strDict);
                                sortedPerLanguage.Add(lang.Key, commandDict);
                            }
                            if (!sortedPerLanguage[lang.Key].ContainsKey(command.Key))
                            {
                                var strDict = new Dictionary<string, string>
                                {
                                    {str.Key, lang.Value}
                                };
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

                // Find Languages
                SupportedLanguages = new List<string>();
                foreach (var lang in sortedPerLanguage)
                {
                    SupportedLanguages.Add(lang.Key);
                }
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Failed to load Translations", e);
                Environment.Exit(GeekbotExitCode.TranslationsFailed.GetHashCode());
            }
        }
        
        private Task<string> GetServerLanguage(ulong guildId)
        {
            try
            {
                string lang;
                try
                {
                    lang = _serverLanguages[guildId];
                    if (!string.IsNullOrEmpty(lang))
                    {
                        return Task.FromResult(lang);
                    }
                    throw new Exception();
                }
                catch
                {
                    lang = _guildSettingsManager.GetSettings(guildId, false)?.Language ?? "EN";
                    _serverLanguages[guildId] = lang;
                    return Task.FromResult(lang);
                }
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Could not get guild language", e);
                return Task.FromResult("EN");
            }
        }

        public async Task<string> GetString(ulong guildId, string command, string stringName)
        {
            var serverLang = await GetServerLanguage(guildId);
            return GetString(serverLang, command, stringName);
        }
        
        public string GetString(string language, string command, string stringName)
        {
            var translation = _translations[language][command][stringName];
            if (!string.IsNullOrWhiteSpace(translation)) return translation;
            translation = _translations[command][stringName]["EN"];
            if (string.IsNullOrWhiteSpace(translation))
            {
                _logger.Warning(LogSource.Geekbot, $"No translation found for {command} - {stringName}");
            }
            return translation;
        }

        private async Task<Dictionary<string, string>> GetDict(ICommandContext context)
        {
            try
            {
                var command = context.Message.Content.Split(' ').First().TrimStart('!').ToLower();
                var serverLanguage = await GetServerLanguage(context.Guild?.Id ?? 0);
                return _translations[serverLanguage][command];
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "No translations for command found", e);
                return new Dictionary<string, string>();
            }
        }

        public async Task<TranslationGuildContext> GetGuildContext(ICommandContext context)
        {
            var dict = await GetDict(context);
            var language = await GetServerLanguage(context.Guild?.Id ?? 0);
            return new TranslationGuildContext(this, language, dict);
        }
        
        public async Task<Dictionary<string, string>> GetDict(ICommandContext context, string command)
        {
            try
            {
                var serverLanguage = await GetServerLanguage(context.Guild?.Id ?? 0);
                return _translations[serverLanguage][command];
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "No translations for command found", e);
                return new Dictionary<string, string>();    
            }
        }

        public async Task<bool> SetLanguage(ulong guildId, string language)
        {
            try
            {
                if (!SupportedLanguages.Contains(language)) return false;
                var guild = _guildSettingsManager.GetSettings(guildId);
                guild.Language = language;
                await _guildSettingsManager.UpdateSettings(guild);
                _serverLanguages[guildId] = language;
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(LogSource.Geekbot, "Error while changing language", e);
                return false;
            }
        }

        public List<string> SupportedLanguages { get; private set; }
    }
}