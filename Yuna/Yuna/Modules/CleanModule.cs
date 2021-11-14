using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yuna.Handlers;
using Yuna.Services;

namespace Yuna.Modules
{
    public class CleanModule : ModuleBase<SocketCommandContext>
    {
        [Command("delete", RunMode = RunMode.Async), Alias("clean")]
        [Name("Clean"), Summary("Deletes messages in the channel.")]
        public async Task ClearCommand(int num)
        {
            SocketGuildUser user = (SocketGuildUser)Context.User;
            if(!user.GuildPermissions.ManageMessages)
            {
                var items = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                await ((ITextChannel)Context.Channel).DeleteMessagesAsync(items);
                var m = await ReplyAsync(
                    embed: await EmbedHandler.ErrorEmbed("⚠️ You don't have permission to execute this command!"));
                const int delay = 4000;
                await Task.Delay(delay);
                await m.DeleteAsync();
            }
            else
            {
                IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(num + 1).FlattenAsync();
                await ((ITextChannel)Context.Channel).DeleteMessagesAsync(messages);
                const int delay = 2000;
                var m = await ReplyAsync(
                    embed: await EmbedHandler.BasicEmbed("Cleaning is complete.", $"{num} have been deleted.", Color.Green));
                await LogService.LogInfoAsync("CLEAN", $"{num} messages have been deleted!");
                await Task.Delay(delay);
                await m.DeleteAsync();
            }
        }
    }
}
