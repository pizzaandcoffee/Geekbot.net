using System.Collections.Generic;
using Discord.WebSocket;
using Geekbot.net.Lib;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Geekbot.net.WebApi
{
    [Route("v1/user")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IDatabase _redis;
        private readonly DiscordSocketClient _client;

        public UserController()
        {
            _client = Program._servicesProvider.GetService(typeof(DiscordSocketClient)) as DiscordSocketClient;
            _redis = Program._servicesProvider.GetService(typeof(IDatabase)) as IDatabase;
            _userRepository = Program._servicesProvider.GetService(typeof(IUserRepository)) as IUserRepository;
        }

        [HttpGet("{id}")]
        public IActionResult getUserInfo(ulong id)
        {
            var user = _userRepository.Get(id);
            if (user.Username == null)
            {
                return NotFound();
            }
            var userGuilds = new List<IUserResponseGuilds>();
            
            var guilds = _client.Guilds;

            foreach (var guild in guilds)
            {
                int.TryParse(_redis.HashGet($"{guild.Id}:Messages", user.Id), out int messages);
                int.TryParse(_redis.HashGet($"{guild.Id}:Karma", user.Id), out int karma);
                if (messages > 0)
                {
                    userGuilds.Add(new IUserResponseGuilds()
                    {
                        Id = guild.Id,
                        Name = guild.Name,
                        Icon = guild.IconUrl,
                        Messages = messages,
                        Level = LevelCalc.GetLevelAtExperience(messages),
                        Karma = karma
                    }); 
                }
            }
            
            var response = new IUserResponse()
            {
                Id = user.Id,
                Username = user.Username,
                Discriminator = user.Discriminator,
                AvatarUrl = user.AvatarUrl,
                IsBot = user.IsBot,
                Guilds = userGuilds
            };
            
            return Ok(response);
        }

        private class IUserResponse
        {
            public ulong Id { get; set; }
            public string Username { get; set; }
            public string Discriminator { get; set; }
            public string AvatarUrl { get; set; }
            public bool IsBot { get; set; }
            public List<IUserResponseGuilds> Guilds { get; set; }
        }

        private class IUserResponseGuilds
        {
            public ulong Id { get; set; }
            public string Name { get; set; }
            public string Icon { get; set; }
            public int Messages { get; set; }
            public int Level { get; set; }
            public int Karma { get; set; }
            public string Href { get; set; }
        }
    }
}