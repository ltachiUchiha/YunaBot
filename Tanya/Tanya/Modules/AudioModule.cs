using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Tanya.Services;

namespace Tanya.Modules
{
    public class AudioModule : ModuleBase<SocketCommandContext>
    {
        public LavaLinkAudioService AudioService { get; set; }

        // We pass an Audio service task to a section that usually requires embedding, since this is what all audio Service tasks return.

        [Command("join")]
        [Name("Join"), Summary("Connects a bot to a voice channel.")]
        public async Task JoinAndPlay()
            => await ReplyAsync(
                embed: await AudioService.JoinAsync(Context.Guild, Context.User as IVoiceState, Context.Channel as ITextChannel));

        [Command("play")]
        [Name("Play"), Summary("Plays a song with the given name or url.")]
        public async Task Play([Remainder]string search)
            => await ReplyAsync(
                embed: await AudioService.PlayAsync(Context.User as SocketGuildUser, Context.Guild, search, Context.User as IVoiceState, Context.Channel as ITextChannel));

        [Command("leave")]
        [Name("Leave"), Summary("Disconnects the bot from the voice channel.")]
        public async Task Leave()
            => await ReplyAsync(
                embed: await AudioService.LeaveAsync(Context.Guild));

        [Command("pause")]
        [Name("Pause"), Summary("Pauses the current track.")]
        public async Task Pause()
            => await ReplyAsync(
                embed: await AudioService.PauseAsync(Context.Guild));

        [Command("resume")]
        [Name("Resume"), Summary("Resumes the current track.")]
        public async Task Resume()
            => await ReplyAsync(
                embed: await AudioService.ResumeAsync(Context.Guild));

        [Command("stop")]
        [Name("Stop"), Summary("Stops playing music and clears the playlist.")]
        public async Task Stop()
            => await ReplyAsync(
                embed: await AudioService.StopAsync(Context.Guild));

        [Command("skip")]
        [Name("Skip"), Summary("Skips the current track.")]
        public async Task Skip()
            => await ReplyAsync(
                embed: await AudioService.SkipAsync(Context.Guild));

        [Command("list")]
        [Name("List"), Summary("Lists tracks.")]
        public async Task List()
            => await ReplyAsync(
                embed: await AudioService.ListAsync(Context.Guild));

        [Command("remove")]
        [Name("Remove"), Summary("Removes a specific record from the sheet.")]
        public async Task Remove(int index)
            => await ReplyAsync(
                embed: await AudioService.RemoveAsync(Context.Guild, index));

        [Command("loop")]
        [Name("Loop"), Summary("Loops the current track.")]
        public async Task Loop()
            => await ReplyAsync(
                embed: await AudioService.LoopAsync(Context.Guild));

        [Command("looplist")]
        [Name("LoopList"), Summary("Loops all list.")]
        public async Task LoopListAsync()
    => await ReplyAsync(
        embed: await AudioService.LoopListAsync(Context.Guild));

        [Command("volume")]
        [Name("Volume"), Summary("Changes the volume of the bot (from 0 to 150).")]
        public async Task Volume(int volume)
            => await ReplyAsync(
                embed: await AudioService.VolumeAsync(Context.Guild, volume));
    }
}
