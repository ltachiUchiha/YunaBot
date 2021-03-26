using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Tanya.Handlers;
using Victoria;

namespace Tanya.Services
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _commandHandler;
        private readonly ServiceProvider _services;
        private readonly LavaNode _lavaNode;
        private readonly LavaLinkAudioService _audioService;
        private readonly CreateConfig _globalData;


        public DiscordService()
        {
            _services = ConfigureServices();
            _client = _services.GetRequiredService<DiscordSocketClient>();
            _commandHandler = _services.GetRequiredService<CommandHandler>();
            _lavaNode = _services.GetRequiredService<LavaNode>();
            _globalData = _services.GetRequiredService<CreateConfig>();
            _audioService = _services.GetRequiredService<LavaLinkAudioService>();

            SubscribeLavaLinkEvents();
            SubscribeDiscordEvents();
        }

        // Initialize the Discord Client.
        public async Task InitializeAsync()
        {
            await InitializeGlobalDataAsync();

            await _client.LoginAsync(TokenType.Bot, CreateConfig.Config.DiscordToken);
            await _client.StartAsync();

            await _commandHandler.InitializeAsync();

            await Task.Delay(-1);
        }

        // Hook Any Client Events Up Here.
        private void SubscribeLavaLinkEvents()
        {
            _lavaNode.OnLog += LogAsync;
            _lavaNode.OnTrackEnded += _audioService.TrackEnded;
        }

        private void SubscribeDiscordEvents()
        {
            _client.Ready += ReadyAsync;
            _client.Log += LogAsync;
        }

        private async Task InitializeGlobalDataAsync()
        {
            await _globalData.InitializeAsync();
        }

        // Used when the Client Fires the ReadyEvent.
        private async Task ReadyAsync()
        {
            try
            {
                await _lavaNode.ConnectAsync();
                //await _client.SetGameAsync(GlobalData.Config.Status);
                await _client.SetActivityAsync(new Game (CreateConfig.Config.Status, CreateConfig.Config.Activity));
            }
            catch (Exception ex)
            {
                await LogService.LogInfoAsync(ex.Source, ex.Message);
            }

        }

        // Used whenever we want to log something to the Console. 
        private async Task LogAsync(LogMessage logMessage)
        {
            await LogService.LogAsync(logMessage.Source, logMessage.Severity, logMessage.Message);
        }

        // Configure our Services for Dependency Injection.
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<LavaNode>()
                .AddSingleton(new LavaConfig())
                .AddSingleton<LavaLinkAudioService>()
                .AddSingleton<CreateConfig>()
                .BuildServiceProvider();
        }

    }
}
