using Discord;
using System.Threading.Tasks;

namespace Yuna.Handlers
{
    public static class EmbedHandler
    {
        // This file is where we can store all the Embed Helper Tasks.
        public static async Task<Embed> BasicEmbed(string title, string description, Color color)
        {
            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color).Build()));
            return embed;
        }

        public static async Task<Embed> ErrorEmbed(string error)
        {
            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle("Error")
                .WithDescription($"\n{error}")
                .WithColor(Color.Red).Build());
            return embed;
        }
    }
}
