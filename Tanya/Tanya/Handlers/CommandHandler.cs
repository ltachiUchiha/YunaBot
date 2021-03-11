using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Tanya.Handlers
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        // Get Everything we need from DI.
        public CommandHandler(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            HookEvents();
        }

        // Initialize the CommandService.
        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(),
                services: _services);
        }

        // Hook Command Specific Events.
        public void HookEvents()
        {
            _commands.CommandExecuted += CommandExecutedAsync;
            _commands.Log += LogAsync;
            _client.MessageReceived += HandleCommandAsync;
        }

        private Task HandleCommandAsync(SocketMessage socketMessage)
        {
            var argPos = 0;
            //Check that the message is a valid command, ignore everything we don't care about. (Private message, messages from other Bots, Etc)
            if (!(socketMessage is SocketUserMessage message) || message.Author.IsBot || message.Author.IsWebhook || message.Channel is IPrivateChannel)
                return Task.CompletedTask;

            // Check that the message has our Prefix
            if (!message.HasStringPrefix(CreateConfig.Config.DefaultPrefix, ref argPos))
                return Task.CompletedTask;

            // Create the CommandContext for use in modules.
            var context = new SocketCommandContext(_client, socketMessage as SocketUserMessage);

            /* Check if the channel ID that the message was sent from is in our Config - Blacklisted Channels. */
            var blacklistedChannelCheck = from a in CreateConfig.Config.BlacklistedChannels
                                          where a == context.Channel.Id
                                          select a;
            var blacklistedChannel = blacklistedChannelCheck.FirstOrDefault();

            /* If the Channel ID is in the list of blacklisted channels. Ignore the command. */
            if (blacklistedChannel == context.Channel.Id)
            {
                return Task.CompletedTask;
            }
            else
            {
                var result = _commands.ExecuteAsync(context, argPos, _services, MultiMatchHandling.Best);
                //
                /* Report any errors if the command didn't execute succesfully. */
                //if (!result.Result.IsSuccess)
                //{
                //    context.Channel.SendMessageAsync(result.Result.ErrorReason);
                //}

                /* If everything worked fine, command will run. */
                return result;
            }
        }

//#pragma warning disable CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
//#pragma warning restore CS1998 // В асинхронном методе отсутствуют операторы await, будет выполнен синхронный метод
        {
            // command is unspecified when there was a search failure (command not found)
            if (!command.IsSpecified)
                return;

            // the command was succesful, we don't care about this result
            if (result.IsSuccess)
                return;
            //TODO: delete this heresy 
            //the command failed, let's notify the user that something happened. 
            await context.Channel.SendMessageAsync($"error: {result}");
        }

        // Used whenever we want to log something to the Console. 
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }
    }
}
