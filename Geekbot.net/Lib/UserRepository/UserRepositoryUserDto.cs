using System;
using System.Collections.Generic;

namespace Geekbot.net.Lib.UserRepository
{
    public class UserRepositoryUser
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsBot { get; set; }
        public DateTimeOffset Joined { get; set; }
        public List<string> UsedNames { get; set; }
    }
}