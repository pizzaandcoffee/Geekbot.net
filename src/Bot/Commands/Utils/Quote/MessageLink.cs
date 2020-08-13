using System;
using System.Data;
using System.Text.RegularExpressions;

namespace Geekbot.Bot.Commands.Utils.Quote
{
    public class MessageLink
    {
        public readonly static Regex re = new Regex(
            @"https:\/\/((canary|ptb)\.)?discord(app)?.com\/channels\/(?<GuildId>\d{16,20})\/(?<ChannelId>\d{16,20})\/(?<MessageId>\d{16,20})",
            RegexOptions.Compiled | RegexOptions.IgnoreCase,
            new TimeSpan(0, 0, 2));
        
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
            
        public MessageLink(string url)
        {
            var matches = re.Matches(url);

            foreach (Match match in matches)
            {
                foreach (Group matchGroup in match.Groups)
                {
                    switch (matchGroup.Name)
                    {
                        case "GuildId":
                            GuildId = ulong.Parse(matchGroup.Value);
                            break;
                        case "ChannelId":
                            ChannelId = ulong.Parse(matchGroup.Value);
                            break;
                        case "MessageId":
                            MessageId = ulong.Parse(matchGroup.Value);
                            break;
                    }
                }
            }
        }

        public static bool IsValid(string link)
        {
            return re.IsMatch(link);
        }
    }
}