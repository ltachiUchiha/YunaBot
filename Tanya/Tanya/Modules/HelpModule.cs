using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Tanya.Handlers;

namespace Tanya.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {

        [Command("help")]
        [Name("Help"), Summary("Displays a list of all commands.")]
        public async Task HelpCommand()
        {
            await Context.User.SendMessageAsync(
                embed: await EmbedHandler.BasicEmbed("Команды:", 
                "**Общие:**\n" +
                "help - Показывает список всех команд.\n " +
                "delete, clean - Удаляет сообщения в канале.\n " + 
                $"Префикс: {CreateConfig.Config.DefaultPrefix}\n\n" +
                "**Музыка:**\n" + 
                "join - Подключает бота к голосовому каналу.\n " +
                "leave - Отключает бота от голосового канала.\n " +
                "play - Воспроизводит песню с заданным именем или url-адресом.\n " +
                "pause - Ставит на паузу текущий трек.\n" +
                "resume - Возобновляет текущий трек.\n" +
                "stop - Перестает играть музыку и очищает плейлист.\n " +
                "skip - Пропускает текущий трек.\n" +
                "list -  Выводит список треков.\n " + 
                "remove  -  Удаляет определенную запись из листа.\n" +
                "loop - Зацикливает текущий трек.\n" +
                "looplist - Зацикливает весь лист.\n" +
                "volume -  Изменяет громкость бота (От 0 до 150).", CreateConfig.Config.Tanya));
            var items = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(items);
        }
    }
}
