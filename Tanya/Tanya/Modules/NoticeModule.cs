using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Tanya.Handlers;

namespace Tanya.Modules
{
    public class NoticeModule : ModuleBase<SocketCommandContext>
    {
        //TODO Redo! "Note to self: Does it work? Don't touch it"
        readonly Color Tanya = new Color(227, 117, 108);

        [Command("tournament"), Alias("tour")]
        [Name("Tournament"), Summary("Creating a message with an invitation to the tournament.")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Tournament(string all, string name, string date, string hour, SocketGuildUser user)
        {
            if (all == "Да" || all == "да")
            {
                var items = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(items);
                var message = await Context.Channel.SendMessageAsync("<@&784892795059961910>");
                var embed = await EmbedHandler.NoticeEmbed($"**{date} Турнир: {name}**", $"Товарищи! {date}, {hour} будет проведен турнир по \"{name}\". \nДля записи и по всем прочим вопросам связанным с турниром обращаться к {user.Mention}.", Tanya);
                await message.ModifyAsync(message => message.Embed = embed);
            }
            else if (all == "Нет" || all == "нет")
            {
                var items = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(items);
                await ReplyAsync(
                    embed: await EmbedHandler.NoticeEmbed($"**{date} Турнир: {name}**", $"Товарищи! {date}, {hour} будет проведен турнир по \"{name}\". \nДля записи и по всем прочим вопросам связанным с турниром обращаться к {user.Mention}.", Tanya));
            }
        }

        [Command("anime")]
        [Name("Anime"), Summary("Create an invitation message to watch anime.")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Anime(string all, string name, string date, string hour)
        {
            if (all == "Да" || all == "да")
            {
                var items = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(items);
                var message = await Context.Channel.SendMessageAsync("<@&784892795059961910>");
                var embed = await EmbedHandler.NoticeEmbed($"**{date} Аниме: {name}**", $"Товарищи! {date}, {hour} будет проведен марафон аниме: \"{name}\".", Tanya);
                await message.ModifyAsync(message => message.Embed = embed);
            }
            else if (all == "Нет" || all == "нет")
            {
                var items = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(items);
                await ReplyAsync(
                    embed: await EmbedHandler.NoticeEmbed($"**{date} Аниме: {name}**", $"Товарищи! {date}, {hour} будет проведен марафон аниме: \"{name}\".", Tanya));
            }
        }

        [Command("mod")]
        [Name("Mod"), Summary("Create a message inviting you to read the mod for Everlasting Summer.")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Mod(string all, string name, string date, string hour, SocketGuildUser user)
        {
            if (all == "Да" || all == "да")
            {
                var items = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(items);
                var message = await Context.Channel.SendMessageAsync("<@&784892795059961910>");
                var embed = await EmbedHandler.NoticeEmbed($"**{date} Мод: {name}**", $"Товарищи! {date}, {hour} будет проведен марафон мода: \"{name}\". Читает: {user.Mention}.", Tanya);
                await message.ModifyAsync(message => message.Embed = embed);
            }
            else if (all == "Нет" || all == "нет")
            {
                var items = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(items);
                await ReplyAsync(
                    embed: await EmbedHandler.NoticeEmbed($"**{date} Мод: {name}**", $"Товарищи! {date}, {hour} будет проведен марафон мода: \"{name}\". Читает: {user.Mention}.", Tanya));
            }
        }

        [Command("film"), Alias("movie")]
        [Name("Movie"), Summary("Create a message inviting you to watch a movie.")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Movie(string all, string name, string date, string hour)
        {
            if (all == "Да" || all == "да")
            {
                var items = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(items);
                var message = await Context.Channel.SendMessageAsync("<@&784892795059961910>");
                var embed = await EmbedHandler.NoticeEmbed($"**{date} Фильм: {name}**", $"Товарищи! {date}, {hour} будет проведен кинопоказ: \"{name}\".", Tanya);
                await message.ModifyAsync(message => message.Embed = embed);
            }
            else if (all == "Нет" || all == "нет")
            {
                var items = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(items);
                await ReplyAsync(
                    embed: await EmbedHandler.NoticeEmbed($"**{date} Фильм: {name}**", $"Товарищи! {date}, {hour} будет проведен кинопоказ: \"{name}\".", Tanya));
            }
        }
    }
}
