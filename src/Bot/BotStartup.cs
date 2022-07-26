using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.Bot.Handlers;
using Geekbot.Core;
using Geekbot.Core.Database;
using Geekbot.Core.GlobalSettings;
using Geekbot.Core.GuildSettingsManager;
using Geekbot.Core.Logger;
using Geekbot.Core.Logger.Adapters;
using Geekbot.Core.ReactionListener;
using Geekbot.Core.UserRepository;
using Microsoft.Extensions.DependencyInjection;

namespace Geekbot.Bot;

public class BotStartup
{
    private readonly IServiceCollection _serviceCollection;
    private readonly GeekbotLogger _logger;
    private readonly RunParameters _runParameters;
    private readonly IGlobalSettings _globalSettings;
    private DiscordSocketClient _client;

    public BotStartup(IServiceCollection serviceCollection, GeekbotLogger logger, RunParameters runParameters, IGlobalSettings globalSettings)
    {
        _serviceCollection = serviceCollection;
        _logger = logger;
        _runParameters = runParameters;
        _globalSettings = globalSettings;
    }

    public async Task Start()
    {
        _logger.Information(LogSource.Geekbot, "Connecting to Discord");
        SetupDiscordClient();
        await Login();
        await _client.SetGameAsync(_globalSettings.GetKey("Game"));
        _logger.Information(LogSource.Geekbot, $"Now Connected as {_client.CurrentUser.Username} to {_client.Guilds.Count} Servers");
        
        _logger.Information(LogSource.Geekbot, "Registering Gateway Handlers");
        await RegisterHandlers();
        
        _logger.Information(LogSource.Geekbot, "Done and ready for use");
        await Task.Delay(-1);
    }
    
    private void SetupDiscordClient()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.DirectMessageReactions |
                             GatewayIntents.DirectMessages |
                             GatewayIntents.GuildMessageReactions |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.GuildWebhooks |
                             GatewayIntents.GuildIntegrations |
                             GatewayIntents.GuildEmojis |
                             GatewayIntents.GuildBans |
                             GatewayIntents.Guilds |
                             GatewayIntents.GuildMembers,
            LogLevel = LogSeverity.Verbose,
            MessageCacheSize = 1000,
        });
            
        var discordLogger = new DiscordLogger(_logger);
        _client.Log += discordLogger.Log;
    }
    
    private async Task Login()
    {
        try
        {
            var token = await GetToken();
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            while (!_client.ConnectionState.Equals(ConnectionState.Connected)) await Task.Delay(25);
        }
        catch (Exception e)
        {
            _logger.Error(LogSource.Geekbot, "Could not connect to Discord", e);
            Environment.Exit(GeekbotExitCode.CouldNotLogin.GetHashCode());
        }
    }
    
    private async Task<string> GetToken()
    {
        var token = _runParameters.Token ?? _globalSettings.GetKey("DiscordToken");
        if (string.IsNullOrEmpty(token))
        {
            Console.Write("Your bot Token: ");
            var newToken = Console.ReadLine();
            await _globalSettings.SetKey("DiscordToken", newToken);
            await _globalSettings.SetKey("Game", "Ping Pong");
            token = newToken;
        }

        return token;
    }
    
    private async Task RegisterHandlers()
    {
        var applicationInfo = await _client.GetApplicationInfoAsync();

        _serviceCollection.AddSingleton<DiscordSocketClient>(_client);
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        
        var commands = new CommandService();
        await commands.AddModulesAsync(Assembly.GetAssembly(typeof(BotStartup)), serviceProvider);
            
        var commandHandler = new CommandHandler(_client, _logger, serviceProvider, commands, applicationInfo, serviceProvider.GetService<IGuildSettingsManager>());
        var userHandler = new UserHandler(serviceProvider.GetService<IUserRepository>(), _logger, serviceProvider.GetService<DatabaseContext>(), _client);
        var reactionHandler = new ReactionHandler(serviceProvider.GetService<IReactionListener>());
        var statsHandler = new StatsHandler(_logger, serviceProvider.GetService<DatabaseContext>());
        var messageDeletedHandler = new MessageDeletedHandler(serviceProvider.GetService<DatabaseContext>(), _logger, _client);
                    
        _client.MessageReceived += commandHandler.RunCommand;
        _client.MessageDeleted += messageDeletedHandler.HandleMessageDeleted;
        _client.UserJoined += userHandler.Joined;
        _client.UserUpdated += userHandler.Updated;
        _client.UserLeft += userHandler.Left;
        _client.ReactionAdded += reactionHandler.Added;
        _client.ReactionRemoved += reactionHandler.Removed;
        if (!_runParameters.InMemory) _client.MessageReceived += statsHandler.UpdateStats;
    }
}