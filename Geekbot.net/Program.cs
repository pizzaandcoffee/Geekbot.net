using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib;
using Geekbot.net.Modules;
using RestSharp;
using StackExchange.Redis;

namespace Geekbot.net
{
    class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private IDatabase redis;
        private RedisValue token;
        private IServiceCollection services;
        private IServiceProvider servicesProvider;

        private static void Main(string[] args)
        {
            Console.WriteLine(@"  ____ _____ _____ _  ______   ___ _____");
            Console.WriteLine(@" / ___| ____| ____| |/ / __ ) / _ \\_  _|");
            Console.WriteLine(@"| |  _|  _| |  _| | ' /|  _ \| | | || |");
            Console.WriteLine(@"| |_| | |___| |___| . \| |_) | |_| || |");
            Console.WriteLine(@" \____|_____|_____|_|\_\____/ \___/ |_|");
            Console.WriteLine("=========================================");
            Console.WriteLine("Starting...");

            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();

            try
            {
                var redisMultiplexer = ConnectionMultiplexer.Connect("127.0.0.1:6379");
                redis = redisMultiplexer.GetDatabase(6);
            }
            catch (Exception)
            {
                Console.WriteLine("Start Redis pls...");
                Environment.Exit(1);
            }

            token = redis.StringGet("discordToken");
            if (token.IsNullOrEmpty)
            {
                Console.Write("Your bot Token: ");
                var newToken = Console.ReadLine();
                redis.StringSet("discordToken", newToken);
                token = newToken;

                Console.Write("Bot Owner User ID: ");
                var ownerId = Console.ReadLine();
                redis.StringSet("botOwner", ownerId);
            }

            services = new ServiceCollection();
            var RandomClient = new Random();
            services.AddSingleton(RandomClient);
            services.AddSingleton(redis);

            Console.WriteLine("Connecting to Discord...");

            await Login();

            await Task.Delay(-1);
        }

        public async Task Login()
        {
            try
            {
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();
                var isConneted = await isConnected();
                if (isConneted)
                {
                    await client.SetGameAsync("Ping Pong");
                    Console.WriteLine($"Now Connected to {client.Guilds.Count} Servers");

                    Console.WriteLine("Registering Stuff");

                    client.MessageReceived += HandleCommand;
                    client.MessageReceived += HandleMessageReceived;
                    client.UserJoined += HandleUserJoined;
                    await commands.AddModulesAsync(Assembly.GetEntryAssembly());
                    servicesProvider = services.BuildServiceProvider();

                    Console.WriteLine("Done and ready for use...\n");
                }
            }
            catch (AggregateException)
            {
                Console.WriteLine("Could not connect to discord...");
                Environment.Exit(1);
            }
        }

        public async Task<bool> isConnected()
        {
            while (!client.ConnectionState.Equals(ConnectionState.Connected))
            {
                await Task.Delay(25);
            }
            return true;
        }

        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            if (message.Author.IsBot) return;
            int argPos = 0;
            var lowCaseMsg = message.ToString().ToLower();
            if (lowCaseMsg.StartsWith("ping"))
            {
                await message.Channel.SendMessageAsync("pong");
                return;
            }
            if (lowCaseMsg.StartsWith("hui"))
            {
                await message.Channel.SendMessageAsync("hui!!!");
                return;
            }
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            var context = new CommandContext(client, message);
            Task.Run(async () => await commands.ExecuteAsync(context, argPos, servicesProvider));
        }

        public async Task HandleMessageReceived(SocketMessage messsageParam)
        {
            var message = messsageParam;
            if (message == null) return;

            var channel = (SocketGuildChannel)message.Channel;

            Console.WriteLine(channel.Guild.Name + " - " + message.Channel + " - " + message.Author.Username + " - " + message.Content);

            var statsRecorder = new StatsRecorder(message, redis);
            Task.Run(async () => await statsRecorder.UpdateUserRecordAsync());
            Task.Run(async () => await statsRecorder.UpdateGuildRecordAsync());
        }

        public async Task HandleUserJoined(SocketGuildUser user)
        {
            if (!user.IsBot)
            {
                var message = redis.StringGet(user.Guild.Id + "-welcomeMsg");
                if (!message.IsNullOrEmpty)
                {
                    message = message.ToString().Replace("$user", user.Mention);
                    await user.Guild.DefaultChannel.SendMessageAsync(message);
                }
            }
        }
    }
}
