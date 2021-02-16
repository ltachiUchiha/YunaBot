using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Tanya.Services;

namespace Tanya.Modules
{
    public class CleanModule : ModuleBase<SocketCommandContext>
    {

        [Command("delete", RunMode = RunMode.Async), Alias("clean")]
        [Name("Clean"), Summary("Deletes messages in the channel.")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task ClearCommand(int num)
        {
            var items = await Context.Channel.GetMessagesAsync(num + 1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(items);

            const int delay = 2000;
            var m = await ReplyAsync($"Очистка завершена. ***{num} было удалено.***");
            await LogService.LogInfoAsync("CLEAN", $"{num} messages were deleted!");
            await Task.Delay(delay);
            await m.DeleteAsync();
        }
    }
}
