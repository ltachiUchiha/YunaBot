using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yuna.Handlers;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace Yuna.Services
{
    public sealed class LavaLinkAudioService
    {
        private readonly LavaNode _lavaNode;

        private LavaTrack track;
        private LavaTrack currentTrack;
        public bool loop = false;
        public bool looplist = false;
        public bool check = false;
        public bool leave = false;
        public bool skip = false;

        public LavaLinkAudioService(LavaNode lavaNode)
            => _lavaNode = lavaNode;
        public async Task<Embed> JoinAsync(IGuild guild, IVoiceState voiceState, ITextChannel textChannel)
        {
            if (_lavaNode.HasPlayer(guild))
            {
                var player = _lavaNode.GetPlayer(guild);
                if (player.VoiceChannel == voiceState.VoiceChannel)
                {
                    return await EmbedHandler.ErrorEmbed("⚠️ I'm already connected to the voice channel!");
                }
                else
                {
                    return await EmbedHandler.ErrorEmbed("⚠️ I'm already connected to a different voice channel!");
                }
            }
            if (voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ You must be connected to a voice channel!");
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, textChannel);
                return await EmbedHandler.BasicEmbed("", $"✅ Joined  \"{voiceState.VoiceChannel.Name}\".", Color.Green);
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> PlayAsync(SocketGuildUser user, IGuild guild, string query, IVoiceState voiceState, ITextChannel textChannel)
        {
            if (user.VoiceChannel == null)
            {
                return await EmbedHandler.ErrorEmbed("You Must First Join a Voice Channel.");
            }

            //Check the guild has a player available.
            if (!_lavaNode.HasPlayer(guild))
            {
                return await EmbedHandler.BasicEmbed("Music", "I'm not connected to a voice channel.", Color.DarkRed);
            }

            try
            {
                var player = _lavaNode.GetPlayer(guild);

                var search = Uri.IsWellFormedUriString(query, UriKind.Absolute) ?
                    await _lavaNode.SearchAsync(query)
                    : await _lavaNode.SearchYouTubeAsync(query);

                if (search.LoadStatus == LoadStatus.NoMatches)
                {
                    return await EmbedHandler.ErrorEmbed($"⚠️ I wasn't able to find anything for \"{query}\".");
                }
                else if (search.LoadStatus == LoadStatus.PlaylistLoaded)
                {
                    for (int trackNumber = 0; trackNumber < search.Tracks.Count; trackNumber++)
                    {
                        track = search.Tracks.ElementAt(trackNumber);
                        if (player.Track != null && player.PlayerState is PlayerState.Playing || player.PlayerState is PlayerState.Paused)
                        {
                            player.Queue.Enqueue(track);
                        }
                        else
                        {
                            if (trackNumber == 0)
                            {
                                await player.PlayAsync(track);
                            }
                            else
                            {
                                player.Queue.Enqueue(track);
                            }   
                        }
                    }
                    return await EmbedHandler.BasicEmbed("🎵 Music", $"The playlist \"{search.Playlist.Name}\" has been successfully added to the queue.", Color.Green);
                }
                else 
                {
                    track = search.Tracks.FirstOrDefault();

                    if (player.Track != null && player.PlayerState is PlayerState.Playing || player.PlayerState is PlayerState.Paused)
                    {
                        player.Queue.Enqueue(track);
                        await LogService .LogInfoAsync("MUSIC", $"\"{track.Title}\" has been added to the music queue.");
                        return await EmbedHandler.BasicEmbed("🎵 Music", $"\"{track.Title}\" has been added to the music queue.", Color.Green);
                    }

                    await player.PlayAsync(track);
                    await LogService.LogInfoAsync("MUSIC", $"Yuna Now Playing: {track.Title}\nUrl: {track.Url}");
                    return await EmbedHandler.BasicEmbed("🎵 Music", $"Now Playing: \"{track.Title}\"\nAuthor: {track.Author}\nUrl: {track.Url}", Color.Green);
                }
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }

        }
        public async Task<Embed> LeaveAsync(IGuild guild, IVoiceState voiceState)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ To execute this command, you must be in the same voice channel as me!");
            }

            try 
            {
                if (player.PlayerState is PlayerState.Playing)
                {
                    looplist = false;
                    loop = false;
                    check = false;
                    player.Queue.Clear();
                    await player.StopAsync();
                }
                leave = true;
                await _lavaNode.LeaveAsync(player.VoiceChannel);

                await LogService.LogInfoAsync("MUSIC", $"Yuna has left.");
                return await EmbedHandler.BasicEmbed("🚫 Music", $"I've left.", Color.Red);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> PauseAsync(IGuild guild, IVoiceState voiceState)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ To execute this command, you must be in the same voice channel as me!");
            }

            try
            {
                if (!(player.PlayerState is PlayerState.Playing))
                {
                    await player.PauseAsync();
                    return await EmbedHandler.BasicEmbed("", "⚠️ There is nothing to pause.", Color.Red);
                }

                await player.PauseAsync();
                return await EmbedHandler.BasicEmbed("", $"⏸️ **Paused:** {player.Track.Title}", ConfigData.Config.Yuna);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> ResumeAsync(IGuild guild, IVoiceState voiceState)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ To execute this command, you must be in the same voice channel as me!");
            }

            try
            {
                if (player.PlayerState is PlayerState.Paused)
                { 
                    await player.ResumeAsync(); 
                }
                return await EmbedHandler.BasicEmbed("", $"▶️ **Resumed:** {player.Track.Title}", Color.Green);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> StopAsync(IGuild guild, IVoiceState voiceState)
        {
            var player = _lavaNode.GetPlayer(guild);

            if (player is null)
                return await EmbedHandler.ErrorEmbed($"Could not aquire player.\nAre you using the bot right now? Check {ConfigData.Config.DefaultPrefix}help for info on how to use the bot.");
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
                return await EmbedHandler.ErrorEmbed("⚠️ To execute this command, you must be in the same voice channel as me!");

            try
            {
                if (player.PlayerState is PlayerState.Playing)
                {
                    looplist = false;
                    loop = false;
                    check = false;
                    player.Queue.Clear();
                    await player.StopAsync();
                }
                await LogService.LogInfoAsync("MUSIC", $"Yuna has stopped playback.");
                return await EmbedHandler.BasicEmbed("", "⏹️ I have stopped playback & the playlist has been cleared.", ConfigData.Config.Yuna);
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> SkipAsync(IGuild guild, IVoiceState voiceState)
        {
            var player = _lavaNode.GetPlayer(guild);
            var сurrenttrack = player.Track;
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ To execute this command, you must be in the same voice channel as me!");
            }

            try
            {
                if (player.Queue.Count < 1)
                {
                    return await EmbedHandler.ErrorEmbed($"⚠️ Unable To skip a track as there is only One or No songs currently playing." +
                        $"\n\nDid you mean {ConfigData.Config.DefaultPrefix}stop?");
                }
                else
                {
                    try
                    {
                        if (loop is true || looplist is true)
                        {
                            if (looplist is true)
                            {
                                check = true;
                                currentTrack = player.Track;
                                player.Queue.Enqueue(currentTrack);
                                await player.SkipAsync();
                                await LogService.LogInfoAsync("MUSIC", $"Skipped: \"{сurrenttrack.Title}\"");
                                return await EmbedHandler.BasicEmbed("", $"⏭️ Skipped: \"{сurrenttrack.Title}\".", ConfigData.Config.Yuna);
                            }
                            check = true;
                            await LogService.LogInfoAsync("MUSIC", $"Skipped: \"{сurrenttrack.Title}\"");
                            await player.SkipAsync();
                            currentTrack = player.Track;
                            skip = true;
                            return await EmbedHandler.BasicEmbed("", $"⏭️ Skipped: \"{сurrenttrack.Title}\".", ConfigData.Config.Yuna);
                        }
                        await LogService.LogInfoAsync("MUSIC", $"Skipped: \"{сurrenttrack.Title}\"");
                        await player.SkipAsync();
                        return await EmbedHandler.BasicEmbed("", $"⏭️ Skipped: \"{сurrenttrack.Title}\".", ConfigData.Config.Yuna);
                    }
                    catch (Exception ex)
                    {
                        return await EmbedHandler.ErrorEmbed(ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> ListAsync(IGuild guild, IVoiceState voiceState)
        {
            var descriptionBuilder = new StringBuilder();
            var player = _lavaNode.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ To execute this command, you must be in the same voice channel as me!");
            }

            try
            {
                if (player.PlayerState is PlayerState.Playing)
                {
                    if (player.Queue.Count < 1 && player.Track != null)
                    {
                        return await EmbedHandler.BasicEmbed($"🎶 Now Playing: {player.Track.Title}", $"Nothing Else Is Queued.", ConfigData.Config.Yuna);
                    }
                    else
                    {
                        var trackNum = 2;
                        foreach (LavaTrack track in player.Queue)
                        {
                            descriptionBuilder.Append($"{trackNum}: {track.Title}\n");
                            trackNum++;
                        }
                        return await EmbedHandler.BasicEmbed("🎶 Playlist", $"Now Playing: [{player.Track.Title}]({player.Track.Url}) \n{descriptionBuilder}", ConfigData.Config.Yuna);
                    }
                }
                else
                {
                    return await EmbedHandler.ErrorEmbed("⚠️ Player doesn't seem to be playing anything right now.");
                }
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }

        }
        public async Task<Embed> NowPlayingAsync(IGuild guild, IVoiceState voiceState)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ To execute this command, you must be in the same voice channel as me!");
            }
            try
            {
                if (player.PlayerState is PlayerState.Playing)
                    return await EmbedHandler.BasicEmbed("🎵 Music", $"Now Playing: \"{track.Title}\"\nAuthor: {track.Author}\nUrl: {track.Url}", ConfigData.Config.Yuna);
                else
                    return await EmbedHandler.ErrorEmbed("⚠️ Nothing Else Is Queued.");
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> LoopAsync(IGuild guild, IVoiceState voiceState)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ To execute this command, you must be in the same voice channel as me!");
            }

            try
            {
                if (looplist is true)
                {
                    return await EmbedHandler.ErrorEmbed("⚠️ Turn off looplist!");
                }
                if (loop is true)
                {
                    loop = false;
                    check = false;
                    await LogService.LogInfoAsync("MUSIC", $"Loop disabled.");
                    return await EmbedHandler.BasicEmbed("", "❌ Loop disabled.", ConfigData.Config.Yuna);
                }

                currentTrack = player.Track;

                loop = true;
                await LogService.LogInfoAsync("MUSIC", $"Loop enabled. Looped track: {currentTrack.Title}");
                return await EmbedHandler.BasicEmbed("", $"🔂 Loop enabled.", ConfigData.Config.Yuna);

            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> LoopListAsync(IGuild guild, IVoiceState voiceState)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ To execute this command, you must be in the same voice channel as me!");
            }

            try
            {
                if (looplist is true)
                {
                    looplist = false;
                    await LogService.LogInfoAsync("MUSIC", $"Looplist disabled.");
                    return await EmbedHandler.BasicEmbed("", "❌ Looplist disabled.", ConfigData.Config.Yuna);
                }
                if (loop is true)
                {
                    return await EmbedHandler.ErrorEmbed("⚠️ Turn off loop!");
                }
                currentTrack = player.Track;
                looplist = true;
                //player.Queue.Enqueue(currentTrack);
                await LogService.LogInfoAsync("MUSIC", $"LoopList enabled. Looped track: {currentTrack.Title}");
                return await EmbedHandler.BasicEmbed("", $"🔂 LoopList enabled.", Color.Green);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> RemoveAsync(IGuild guild, IVoiceState voiceState , int index)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ To execute this command, you must be in the same voice channel as me!");
            }

            index -= 2;

            try
            {
                var trackToRemove = player.Queue.RemoveAt(index);
                await LogService.LogInfoAsync("MUSIC", $"Removed {trackToRemove.Title} from queue");
                return await EmbedHandler.BasicEmbed("", $"❌ Removed \"{trackToRemove.Title}\" from queue.", ConfigData.Config.Yuna);
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }

        }
        public async Task<Embed> VolumeAsync(IGuild guild, IVoiceState voiceState , int volume)
        {
            var player = _lavaNode.GetPlayer(guild);
            if (player.VoiceChannel != voiceState.VoiceChannel || voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ To execute this command, you must be in the same voice channel as me!");
            }

            if (volume > 150 || volume < 0)
            {
                return await EmbedHandler.BasicEmbed("", "⚠️ Volume must be between 0 and 150.", Color.Red);
            }

            try
            {
                await player.UpdateVolumeAsync((ushort)volume);
                await LogService.LogInfoAsync("MUSIC", $"Bot Volume set to: {volume}");
                return await EmbedHandler.BasicEmbed("", $"🔊 Bot Volume set to: {volume}.", ConfigData.Config.Yuna);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task TrackEnded(TrackEndedEventArgs args)
        {
            if (loop is true && check is false)
            {
                await args.Player.PlayAsync(currentTrack);
            }
            if (looplist is true && check is false)
            {
                args.Player.Queue.Enqueue(currentTrack);
                args.Player.Queue.TryDequeue(out LavaTrack track);
                await args.Player.PlayAsync(track);
                currentTrack = args.Player.Track;
            }
            if (check is true)
            {
                check = false;
            }

            else if (loop is false && looplist is false && skip is false && leave is false)
            {
                args.Player.Queue.TryDequeue(out track);
                await args.Player.PlayAsync(track);
            }
            else if (loop is false && looplist is false && skip is true && leave is false)
            {
                skip = false;
                track = (LavaTrack)args.Player.Queue.Skip(1);
                await args.Player.PlayAsync(track);
            }
            if (leave is true) 
            {
                leave = false;
            }
        }
    }
}
