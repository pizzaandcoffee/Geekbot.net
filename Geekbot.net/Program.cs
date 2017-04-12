using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

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
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            commands = new CommandService();

            const string token = "MTgxMDkyOTgxMDUzNDU2Mzg0.C8_UTw.PvXLAVOTccbrWKLMeyvN9WqRPlU";

            map = new DependencyMap();

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

            Console.WriteLine(message.Channel + " - " + message.Author + " - " + message.Content);
        }
    }
}