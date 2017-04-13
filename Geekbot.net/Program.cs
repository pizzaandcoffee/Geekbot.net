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

        private static void Main(string[] args)
        {
            Console.WriteLine("  ____ _____ _____ _  ______   ___ _____");
            Console.WriteLine(" / ___| ____| ____| |/ / __ ) / _ \\_   _|");
            Console.WriteLine("| |  _|  _| |  _| | ' /|  _ \\| | | || |");
            Console.WriteLine("| |_| | |___| |___| . \\| |_) | |_| || |");
            Console.WriteLine(" \\____|_____|_____|_|\\_\\____/ \\___/ |_|");
            Console.WriteLine("=========================================");
            Console.WriteLine("Starting...");
            Task.WaitAll(new Program().MainAsync());
        }

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();

            const string token = "MTgxMDkyOTgxMDUzNDU2Mzg0.C8_UTw.PvXLAVOTccbrWKLMeyvN9WqRPlU";

            map = new DependencyMap();
            map.Add<ICatClient>(new CatClient());

            await InstallCommands();
            Console.WriteLine("Connecting to Discord...");
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            Console.WriteLine("Done and ready for use...\n");

            await Task.Delay(-1);
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;
            client.MessageReceived += HandleMessageReceived;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
        public async Task HandleCommand(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            int argPos = 0;
            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            var context = new CommandContext(client, message);
            var result = await commands.ExecuteAsync(context, argPos, map);
            if (!result.IsSuccess)
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        public async Task HandleMessageReceived(SocketMessage messsageParam)
        {
            var message = messsageParam;
            if (message == null) return;
            if (message.Author.Username.Contains("Geekbot")) return;

            var channel = (SocketGuildChannel) message.Channel;

            Console.WriteLine(channel.Guild.Name + " - " + message.Channel + " - " + message.Author.Username + " - " + message.Content);

            var statsRecorder =  new StatsRecorder(message);
            var updateUserRecordAsync = statsRecorder.UpdateUserRecordAsync();
            var updateGuildRecordAsync = statsRecorder.UpdateGuildRecordAsync();
        }
    }
}