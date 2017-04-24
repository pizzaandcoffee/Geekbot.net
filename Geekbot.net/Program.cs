using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Geekbot.net.Lib;
using Geekbot.net.Modules;

namespace Geekbot.net
{
    class Program
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private DependencyMap map;
        private IRedisClient redis;

        private static void Main(string[] args)
        {
            Console.WriteLine(@"  ____ _____ _____ _  ______   ___ _____");
            Console.WriteLine(@" / ___| ____| ____| |/ / __ ) / _ \\_  _|");
            Console.WriteLine(@"| |  _|  _| |  _| | ' /|  _ \| | | || |");
            Console.WriteLine(@"| |_| | |___| |___| . \| |_) | |_| || |");
            Console.WriteLine(@" \____|_____|_____|_|\_\____/ \___/ |_|");
            Console.WriteLine("=========================================");
            Console.WriteLine("Starting...");

            Task.WaitAll(new Program().MainAsync());
        }

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();
            redis = new RedisClient();

            var token = redis.Client.StringGet("discordToken");
            if (token.IsNullOrEmpty)
            {
                Console.Write("Your bot Token: ");
                var newToken = Console.ReadLine();
                redis.Client.StringSet("discordToken", newToken);
                token = newToken;

                Console.Write("Bot Owner User ID: ");
                var ownerId = Console.ReadLine();
                redis.Client.StringSet("botOwner", ownerId);
            }

            map = new DependencyMap();
            map.Add<ICatClient>(new CatClient());
            map.Add(redis);

            await InstallCommands();
            Console.WriteLine("Connecting to Discord...");
            try
            {
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();
            }
            catch (AggregateException)
            {
                Console.WriteLine("Could not connect to discord...");
                Environment.Exit(1);
            }
            Console.WriteLine("Done and ready for use...\n");

            await Task.Delay(-1);
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            client.MessageReceived += HandleMessageReceived;

            client.UserJoined += HandleUserJoined;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            if (message.Author.Username.Equals(client.CurrentUser.Username)) return;
            int argPos = 0;
            if (message.ToString().ToLower().StartsWith("ping"))
            {
                await message.Channel.SendMessageAsync("pong");
                return;
            }
            if (message.ToString().ToLower().StartsWith("hui"))
            {
                await message.Channel.SendMessageAsync("hui!!!");
                return;
            }
            if (message.ToString().ToLower().Contains("teamspeak") || message.ToString().ToLower().Contains("skype"))
            {
                await message.Channel.SendMessageAsync("How dare you to use such a filthy word in here http://bit.ly/2poL2IZ");
                return;
            }
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            var context = new CommandContext(client, message);
            Task.Run(() => commands.ExecuteAsync(context, argPos, map));
        }

        public async Task HandleMessageReceived(SocketMessage messsageParam)
        {
            var message = messsageParam;
            if (message == null) return;
//            if (message.Author.Username.Equals(client.CurrentUser.Username)) return;

            var channel = (SocketGuildChannel)message.Channel;

            Console.WriteLine(channel.Guild.Name + " - " + message.Channel + " - " + message.Author.Username + " - " + message.Content);

            var statsRecorder = new StatsRecorder(message, redis);
            await statsRecorder.UpdateUserRecordAsync();
            await statsRecorder.UpdateGuildRecordAsync();
        }

        public async Task HandleUserJoined(SocketGuildUser user)
        {
            if (!user.IsBot)
            {
                var message = redis.Client.StringGet(user.Guild.Id + "-welcomeMsg");
                if (!message.IsNullOrEmpty)
                {
                    message = message.ToString().Replace("$user", user.Mention);
                    await user.Guild.DefaultChannel.SendMessageAsync(message);
                }
            }
        }
    }
}
