using Discord;
using System.Collections.Generic;

namespace Yuna.DataStructs
{
    public class Config
    {
        public string DiscordToken { get; set; }
        public string DefaultPrefix { get; set; }
        public string Status { get; set; }
        public ActivityType Activity { get; set; }
        public readonly Color Yuna = new Color(119, 72, 129);
        public List<ulong> BlacklistedChannels { get; set; }
    }
}
