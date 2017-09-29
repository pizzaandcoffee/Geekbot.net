using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Serilog;
using StackExchange.Redis;

namespace Geekbot.net.Lib
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabase _redis;
        private readonly ILogger _logger;
        public UserRepository(IDatabase redis, ILogger logger)
        {
            _redis = redis;
            _logger = logger;
        }

        public Task<bool> Update(SocketUser user)
        {
            try
            {
                _redis.HashSetAsync($"Users:{user.Id}", new HashEntry[]
                {
                    new HashEntry("Id", user.Id.ToString()),
                    new HashEntry("Username", user.Username),
                    new HashEntry("Discriminator", user.Discriminator),
                    new HashEntry("AvatarUrl", user.GetAvatarUrl() ?? "0"),
                    new HashEntry("IsBot", user.IsBot), 
                });
                _logger.Information($"[UserRepository] Updated User {user.Id}");
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                _logger.Warning(e, $"[UserRepository] Failed to update {user.Id}");
                return Task.FromResult(false);
            }
        }

        public UserRepositoryUser Get(ulong userId)
        {
            var user = _redis.HashGetAll($"Users:{userId}").ToDictionary();
            var dto = new UserRepositoryUser();
            foreach (var a in user)
            {
                switch (a.Key)
                {
                    case "Id":
                        dto.Id = ulong.Parse(a.Value);
                        break;
                    case "Username":
                        dto.Username = a.Value.ToString();
                        break;
                    case "Discriminator":
                        dto.Discriminator = a.Value.ToString();
                        break;
                    case "AvatarUrl":
                        dto.AvatarUrl = (a.Value != "0") ? a.Value.ToString() : null;
                        break;
                    case "IsBot":
                        dto.IsBot = a.Value == 1;
                        break;
                }
            }
            return dto;
        }
    }

    public class UserRepositoryUser
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsBot { get; set; }
    }

    public interface IUserRepository
    {
        Task<bool> Update(SocketUser user);
        UserRepositoryUser Get(ulong userId);
    }
}