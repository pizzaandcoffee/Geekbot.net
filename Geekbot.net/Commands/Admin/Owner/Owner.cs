﻿using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Database;
using Geekbot.net.Lib.AlmostRedis;
using Geekbot.net.Lib.ErrorHandling;
using Geekbot.net.Lib.GlobalSettings;
using Geekbot.net.Lib.Logger;
using Geekbot.net.Lib.UserRepository;

namespace Geekbot.net.Commands.Admin.Owner
{
    [Group("owner")]
    [RequireOwner]
    public class Owner : ModuleBase
    {
        private readonly DiscordSocketClient _client;
        private readonly IErrorHandler _errorHandler;
        private readonly DatabaseContext _database;
        private readonly IGlobalSettings _globalSettings;
        private readonly IGeekbotLogger _logger;
        private readonly IAlmostRedis _redis;
        private readonly IUserRepository _userRepository;

        public Owner(IAlmostRedis redis, DiscordSocketClient client, IGeekbotLogger logger, IUserRepository userRepositry, IErrorHandler errorHandler, DatabaseContext database, IGlobalSettings globalSettings)
        {
            _redis = redis;
            _client = client;
            _logger = logger;
            _userRepository = userRepositry;
            _errorHandler = errorHandler;
            _database = database;
            _globalSettings = globalSettings;
        }

        [Command("migrate", RunMode = RunMode.Async)]
        public async Task Migrate(MigrationMethods method, string force = "")
        {
            try
            {
                switch (method)
                {
                    case MigrationMethods.redis:
                        var status = _globalSettings.GetKey("MigrationStatus");
                        if (status.Equals("Running"))
                        {
                            await ReplyAsync("Migration already running");
                            return;
                        }
                        if (status.Equals("Done") && !force.Equals("force"))
                        {
                            await ReplyAsync("Migration already ran, write `!owner migrate redis force` to run again");
                            return;                  
                        }

                        await ReplyAsync("starting migration");
                        await _globalSettings.SetKey("MigrationStatus", "Running");
                        var redisMigration = new RedisMigration(_database, _redis, _logger, _client);
                        await redisMigration.Migrate();
                        await _globalSettings.SetKey("MigrationStatus", "Done");
                        break;
                    
                    case MigrationMethods.messages:
                        await ReplyAsync("Migrating Messages to postgres...");
                        var messageMigration = new MessageMigration(_database, _redis, _logger);
                        await messageMigration.Migrate();
                        break;
                    
                    default:
                        await ReplyAsync("No Migration Method specified...");
                        break;
                }
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }

            await ReplyAsync("done");
        }

        [Command("youtubekey", RunMode = RunMode.Async)]
        [Summary("Set the youtube api key")]
        public async Task SetYoutubeKey([Summary("API Key")] string key)
        {
            await _globalSettings.SetKey("YoutubeKey", key);
            await ReplyAsync("Apikey has been set");
        }

        [Command("game", RunMode = RunMode.Async)]
        [Summary("Set the game that the bot is playing")]
        public async Task SetGame([Remainder] [Summary("Game")] string key)
        {
            await _globalSettings.SetKey("Game", key);
            await _client.SetGameAsync(key);
            _logger.Information(LogSource.Geekbot, $"Changed game to {key}");
            await ReplyAsync($"Now Playing {key}");
        }

        [Command("popuserrepo", RunMode = RunMode.Async)]
        [Summary("Populate user cache")]
        public async Task PopUserRepoCommand()
        {
            var success = 0;
            var failed = 0;
            try
            {
                _logger.Warning(LogSource.UserRepository, "Populating User Repositry");
                await ReplyAsync("Starting Population of User Repository");
                foreach (var guild in _client.Guilds)
                {
                    _logger.Information(LogSource.UserRepository, $"Populating users from {guild.Name}");
                    foreach (var user in guild.Users)
                    {
                        var succeded = await _userRepository.Update(user);
                        var inc = succeded ? success++ : failed++;
                    }
                }

                _logger.Warning(LogSource.UserRepository, "Finished Updating User Repositry");
                await ReplyAsync(
                    $"Successfully Populated User Repository with {success} Users in {_client.Guilds.Count} Guilds (Failed: {failed})");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context,
                    "Couldn't complete User Repository, see console for more info");
            }
        }

        [Command("error", RunMode = RunMode.Async)]
        [Summary("Throw an error un purpose")]
        public async Task PurposefulError()
        {
            try
            {
                throw new Exception("Error Generated by !owner error");
            }
            catch (Exception e)
            {
                await _errorHandler.HandleCommandException(e, Context);
            }
        }
    }
}