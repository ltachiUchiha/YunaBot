using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Yuna.Handlers;

namespace Yuna.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {

        [Command("help")]
        [Name("Help"), Summary("Displays a list of all commands.")]
        public async Task HelpCommand()
        {
            await Context.User.SendMessageAsync(
                embed: await EmbedHandler.BasicEmbed("Commands:",
                "**General:**\n" +
                "help - Shows a list of all commands.\n " +
                "delete, clean - Removes messages in a channel.\n " + 
                $"Prefix: {ConfigData.Config.DefaultPrefix}\n\n" +
                "**Music:**\n" +
                "join - Connects a bot to a voice channel.\n " +
                "leave - Disconnects the bot from the voice channel.\n " +
                "play - Plays a song by a given search or url.\n " +
                "pause - Pauses the current track.\n" +
                "resume - Resumes the current track.\n" +
                "stop - Stops playing music and clears the playlist.\n " +
                "skip - Skips the current track.\n" +
                "list -  Outputs a queue of tracks.\n " +
                "nowplaying -  Displays currently playing music.\n " +
                "remove  -  Removes a specific track from the queue.\n" +
                "loop - Loops the current track.\n" +
                "looplist - Loops the entire queue.\n" +
                "volume -  Changes the volume of the bot (from 0 to 150).", ConfigData.Config.Yuna));
            var items = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(items);
        }
    }
}
