using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Yuna.Services;

namespace Yuna.Modules
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {
        public LavaLinkAudioService AudioService { get; set; }

        // We pass an Audio service task to a section that usually requires embedding, since this is what all audio Service tasks return.

        [Command("join", RunMode = RunMode.Async)]
        [Name("Join"), Summary("Connects a bot to a voice channel.")]
        public async Task JoinAndPlay()
            => await ReplyAsync(
                embed: await AudioService.JoinAsync(Context.Guild, Context.User as IVoiceState, Context.Channel as ITextChannel));

        [Command("play", RunMode = RunMode.Async)]
        [Name("Play"), Summary("Plays a song with the given name or url.")]
        public async Task Play([Remainder]string search)
            => await ReplyAsync(
                embed: await AudioService.PlayAsync(Context.User as SocketGuildUser, Context.Guild, search, Context.User as IVoiceState, Context.Channel as ITextChannel));
        
        [Command("leave", RunMode = RunMode.Async)]
        [Name("Leave"), Summary("Disconnects the bot from the voice channel.")]
        public async Task Leave()
            => await ReplyAsync(
                embed: await AudioService.LeaveAsync(Context.Guild, Context.User as IVoiceState));
        [Command("pause", RunMode = RunMode.Async)]
        [Name("Pause"), Summary("Pauses the current track.")]
        public async Task Pause()
            => await ReplyAsync(
                embed: await AudioService.PauseAsync(Context.Guild, Context.User as IVoiceState));

        [Command("resume", RunMode = RunMode.Async)]
        [Name("Resume"), Summary("Resumes the current track.")]
        public async Task Resume()
            => await ReplyAsync(
                embed: await AudioService.ResumeAsync(Context.Guild, Context.User as IVoiceState));

        [Command("stop", RunMode = RunMode.Async)]
        [Name("Stop"), Summary("Stops playing music and clears the playlist.")]
        public async Task Stop()
            => await ReplyAsync(
                embed: await AudioService.StopAsync(Context.Guild, Context.User as IVoiceState));

        [Command("skip", RunMode = RunMode.Async)]
        [Name("Skip"), Summary("Skips the current track.")]
        public async Task Skip()
            => await ReplyAsync(
                embed: await AudioService.SkipAsync(Context.Guild, Context.User as IVoiceState));

        [Command("list", RunMode = RunMode.Async)]
        [Name("List"), Summary("Lists tracks.")]
        public async Task List()
            => await ReplyAsync(
                embed: await AudioService.ListAsync(Context.Guild, Context.User as IVoiceState));
        [Command("nowplaying", RunMode = RunMode.Async)]
        [Name("List"), Summary("Lists tracks.")]
        public async Task NowPlayng()
            => await ReplyAsync(
                embed: await AudioService.NowPlayingAsync(Context.Guild, Context.User as IVoiceState));

        [Command("remove", RunMode = RunMode.Async)]
        [Name("Remove"), Summary("Removes a specific record from the sheet.")]
        public async Task Remove(int index)
            => await ReplyAsync(
                embed: await AudioService.RemoveAsync(Context.Guild, Context.User as IVoiceState, index));

        [Command("loop", RunMode = RunMode.Async)]
        [Name("Loop"), Summary("Loops the current track.")]
        public async Task Loop()
            => await ReplyAsync(
                embed: await AudioService.LoopAsync(Context.Guild, Context.User as IVoiceState));

        [Command("looplist", RunMode = RunMode.Async)]
        [Name("LoopList"), Summary("Loops all list.")]
        public async Task LoopListAsync()
            => await ReplyAsync(
                embed: await AudioService.LoopListAsync(Context.Guild, Context.User as IVoiceState));

        [Command("volume", RunMode = RunMode.Async)]
        [Name("Volume"), Summary("Changes the volume of the bot (from 0 to 150).")]
        public async Task Volume(int volume)
            => await ReplyAsync(
                embed: await AudioService.VolumeAsync(Context.Guild, Context.User as IVoiceState, volume));
    }
}
