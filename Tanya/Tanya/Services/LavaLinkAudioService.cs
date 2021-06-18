using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tanya.Handlers;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace Tanya.Services
{
    public sealed class LavaLinkAudioService
    {
        private readonly LavaNode _lavaNode;

        private LavaTrack track;
        private LavaTrack currentTrack;
        public bool loop = false;
        public bool looplist = false;
        public bool check = false;

        public LavaLinkAudioService(LavaNode lavaNode)
            => _lavaNode = lavaNode;
        public async Task<Embed> JoinAsync(IGuild guild, IVoiceState voiceState, ITextChannel textChannel)
        {
            if (_lavaNode.HasPlayer(guild))
            {
                return await EmbedHandler.ErrorEmbed("⚠️ Я уже подключена к голосовому каналу!");
            }

            if (voiceState.VoiceChannel is null)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ Вы должны быть подключены к голосовому каналу!");
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, textChannel);
                return await EmbedHandler.BasicEmbed("", $"✅ Присоединилась к \"{voiceState.VoiceChannel.Name}\".", Color.Green);
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> PlayAsync(SocketGuildUser user, IGuild guild, string query, IVoiceState voiceState, ITextChannel textChannel)
        {
            #region Join/Play
            if (_lavaNode.HasPlayer(guild))
            {
                await EmbedHandler.ErrorEmbed("⚠️ Я уже подключилась к голосовому каналу!");
            }

            if (voiceState.VoiceChannel is null)
            {
                await EmbedHandler.ErrorEmbed("⚠️ Вы должны быть подключены к голосовому каналу!");
            }

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, textChannel);
                await EmbedHandler.BasicEmbed("", $"✅ Присоединилась к {voiceState.VoiceChannel.Name}.", Color.Green);
            }
            catch (Exception ex)
            {
                await EmbedHandler.ErrorEmbed(ex.Message);
            }
            #endregion

            if (user.VoiceChannel == null)
            {
                return await EmbedHandler.ErrorEmbed("⚠️ Вы должны быть подключены к голосовому каналу!");
            }

            if (!_lavaNode.HasPlayer(guild))
            {
                return await EmbedHandler.ErrorEmbed("⚠️ Я не подключена к голосовому каналу!");
            }

            var player = _lavaNode.GetPlayer(guild);

            try
            {
                var search = Uri.IsWellFormedUriString(query, UriKind.Absolute) ?
                    await _lavaNode.SearchAsync(query)
                    : await _lavaNode.SearchYouTubeAsync(query);

                if (search.LoadStatus == LoadStatus.NoMatches)
                {
                    return await EmbedHandler.ErrorEmbed($"⚠️ Я не смогла найти \"{query}\".");
                }
                if (search.LoadStatus == LoadStatus.PlaylistLoaded)
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
                    return await EmbedHandler.BasicEmbed("🎵 Музыка", $"Плейлист \"{search.Playlist.Name}\" успешно добавлен в очередь.", Color.Green);
                }
                else
                {
                    track = search.Tracks.FirstOrDefault();

                    if (player.Track != null && player.PlayerState is PlayerState.Playing || player.PlayerState is PlayerState.Paused)
                    {
                        player.Queue.Enqueue(track);
                        await LogService .LogInfoAsync("MUSIC", $"\"{track.Title}\" has been added to the music queue.");
                        if (looplist is true)
                        {
                            return await EmbedHandler.BasicEmbed("🎵 Музыка", $"\"{track.Title}\" добавлен в очередь. \n\nLooplist: {looplist}", Color.Green);
                        }
                        else if (loop is true)
                        {
                            return await EmbedHandler.BasicEmbed("🎵 Музыка", $"\"{track.Title}\" добавлен в очередь. \n\nLoop: {loop}", Color.Green);
                        }
                        return await EmbedHandler.BasicEmbed("🎵 Музыка", $"\"{track.Title}\" добавлен в очередь.", Color.Green);
                    }

                    if (track.IsStream is true)
                    {
                        await player.PlayAsync(track);
                        await LogService .LogInfoAsync("MUSIC", $"Tanya Now Playing: {track.Title}\nUrl: {track.Url}");
                        return await EmbedHandler.BasicEmbed("🎵 Музыка", $"Сейчас Играет: \"{track.Title}\"\nТип: Стрим\nАвтор: {track.Author}\nUrl: {track.Url}", Color.Green);
                    }

                    await player.PlayAsync(track);
                    await LogService.LogInfoAsync("MUSIC", $"Tanya Now Playing: {track.Title}\nUrl: {track.Url}");
                    return await EmbedHandler.BasicEmbed("🎵 Музыка", $"Сейчас Играет: \"{track.Title}\"\nТип: Видео\nДлительность: {track.Duration}\nАвтор: {track.Author}\nUrl: {track.Url}", Color.Green);
                }
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }

        }
        public async Task<Embed> LeaveAsync(IGuild guild)
        {
            var player = _lavaNode.GetPlayer(guild);

            try
            {
                if (player.PlayerState is PlayerState.Playing)
                {
                    await player.StopAsync();
                }

                await _lavaNode.LeaveAsync(player.VoiceChannel);

                await LogService.LogInfoAsync("MUSIC", $"Tanya has left.");
                return await EmbedHandler.BasicEmbed("🚫 Я ушла.", $"Я устала. Я робожук.", Color.Red);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> PauseAsync(IGuild guild)
        {
            var player = _lavaNode.GetPlayer(guild);

            try
            {
                if (!(player.PlayerState is PlayerState.Playing))
                {
                    await player.PauseAsync();
                    return await EmbedHandler.BasicEmbed("", "⚠️ Нечего поставить на паузу.", Color.Red);
                }

                await player.PauseAsync();
                return await EmbedHandler.BasicEmbed("", $"⏸️ Приостановлено: {player.Track.Title}", CreateConfig.Config.Tanya);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> ResumeAsync(IGuild guild)
        {
            var player = _lavaNode.GetPlayer(guild);

            try
            {
                if (player.PlayerState is PlayerState.Paused)
                { 
                    await player.ResumeAsync(); 
                }
                return await EmbedHandler.BasicEmbed("", $"▶️ Возобновлено: {player.Track.Title}", Color.Green);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> StopAsync(IGuild guild)
        {
            var player = _lavaNode.GetPlayer(guild);

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
                await LogService.LogInfoAsync("MUSIC", $"Tanya has stopped playback.");
                return await EmbedHandler.BasicEmbed("", "⏹️ Я остановила воспроизведение и очистила плейлист.", CreateConfig.Config.Tanya);
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> SkipAsync(IGuild guild)
        {
            var player = _lavaNode.GetPlayer(guild);
            var сurrenttrack = player.Track;

            try
            {
                if (player.Queue.Count < 1)
                {
                    return await EmbedHandler.ErrorEmbed($"⚠️ Невозможно пропустить песню, так как в данный момент проигрывается только одна или ни одной песни." +
                        $"\n\nМожет, вы имеете в виду {CreateConfig.Config.DefaultPrefix}stop?");
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
                                await LogService.LogInfoAsync("MUSIC", $"Skipped: \"{сurrenttrack.Title}\"");
                                await player.SkipAsync();
                                return await EmbedHandler.BasicEmbed("", $"⏭️ Я успешно пропустила \"{сurrenttrack.Title}\".", CreateConfig.Config.Tanya);
                            }
                            check = true;
                            await LogService.LogInfoAsync("MUSIC", $"Skipped: \"{сurrenttrack.Title}\"");
                            await player.SkipAsync();
                            currentTrack = player.Track;
                            return await EmbedHandler.BasicEmbed("", $"⏭️ Я успешно пропустила \"{сurrenttrack.Title}\".", CreateConfig.Config.Tanya);
                        }
                        await LogService.LogInfoAsync("MUSIC", $"Skipped: \"{сurrenttrack.Title}\"");
                        await player.SkipAsync();
                        return await EmbedHandler.BasicEmbed("", $"⏭️ Я успешно пропустила \"{сurrenttrack.Title}\".", CreateConfig.Config.Tanya);
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
        public async Task<Embed> ListAsync(IGuild guild)
        {
            var descriptionBuilder = new StringBuilder();
            var player = _lavaNode.GetPlayer(guild);

            try
            {
                if (player.PlayerState is PlayerState.Playing)
                {
                    if (player.Queue.Count < 1 && player.Track != null)
                    {
                        return await EmbedHandler.BasicEmbed($"🎶 Сейчас Играет: {player.Track.Title}", $"Больше ничего не стоит в очереди. \n\nLoop: {loop}", CreateConfig.Config.Tanya);
                    }
                    else
                    {
                        var trackNum = 2;
                        foreach (LavaTrack track in player.Queue)
                        {
                            descriptionBuilder.Append($"{trackNum}: [{track.Title}]({track.Url}) - {track.Id}\n");
                            trackNum++;
                        }
                        if (looplist is true)
                        {
                            return await EmbedHandler.BasicEmbed("🎶 Плейлист", $"Сейчас Играет: [{player.Track.Title}]({player.Track.Url}) \n{descriptionBuilder} \n\nLooplist: {looplist}", CreateConfig.Config.Tanya);
                        }
                        return await EmbedHandler.BasicEmbed("🎶 Плейлист", $"Сейчас Играет: [{player.Track.Title}]({player.Track.Url}) \n{descriptionBuilder} \n\nLoop: {loop}", CreateConfig.Config.Tanya);
                    }
                }
                else
                {
                    return await EmbedHandler.ErrorEmbed("⚠️ Похоже, сейчас ничто не играет.");
                }
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }

        }
        public async Task<Embed> LoopAsync(IGuild guild)
        {
            var player = _lavaNode.GetPlayer(guild);

            try
            {
                if (looplist is true)
                {
                    return await EmbedHandler.ErrorEmbed("⚠️ Выключите looplist!");
                }
                if (loop is true)
                {
                    loop = false;
                    check = false;
                    await LogService.LogInfoAsync("MUSIC", $"Loop disabled.");
                    return await EmbedHandler.BasicEmbed("", "❌ Loop выключен.", CreateConfig.Config.Tanya);
                }

                currentTrack = player.Track;

                loop = true;
                await LogService.LogInfoAsync("MUSIC", $"Loop enabled. Looped track: {currentTrack.Title}");
                return await EmbedHandler.BasicEmbed("", $"🔂 Loop включен.", Color.Green);
                //return await EmbedHandler.BasicEmbed("Музыка", $"Loop включен. \nЗа жопу схватили: \"{currentTrack.Title}\"");

            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> LoopListAsync(IGuild guild)
        {
            var player = _lavaNode.GetPlayer(guild);

            try
            {
                if (looplist is true)
                {
                    looplist = false;
                    await LogService.LogInfoAsync("MUSIC", $"Looplist disabled.");
                    return await EmbedHandler.BasicEmbed("", "❌ Looplist выключен.", CreateConfig.Config.Tanya);
                }
                if (loop is true)
                {
                    return await EmbedHandler.ErrorEmbed("⚠️ Выключите обычный loop!");
                }
                currentTrack = player.Track;
                looplist = true;
                //player.Queue.Enqueue(currentTrack);
                await LogService.LogInfoAsync("MUSIC", $"Loop enabled. Looped track: {currentTrack.Title}");
                return await EmbedHandler.BasicEmbed("", $"🔂 Loop включен.", Color.Green);
            }
            catch (InvalidOperationException ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }
        }
        public async Task<Embed> RemoveAsync(IGuild guild, int index)
        {

            var player = _lavaNode.GetPlayer(guild);

            index -= 2;

            try
            {
                var trackToRemove = player.Queue.RemoveAt(index);
                await LogService.LogInfoAsync("MUSIC", $"Removed {trackToRemove.Title} from queue");
                return await EmbedHandler.BasicEmbed("", $"❌ \"{trackToRemove.Title}\" удалено из листа.", CreateConfig.Config.Tanya);
            }
            catch (Exception ex)
            {
                return await EmbedHandler.ErrorEmbed(ex.Message);
            }

        }
        public async Task<Embed> VolumeAsync(IGuild guild, int volume)
        {
            var player = _lavaNode.GetPlayer(guild);

            if (volume > 150 || volume < 0)
            {
                return await EmbedHandler.BasicEmbed("", "⚠️ Громкость должна быть от 0 до 150.", Color.Red);
            }

            try
            {
                if (volume == 0)
                {
                    await player.UpdateVolumeAsync((ushort)volume);
                    await LogService.LogInfoAsync("MUSIC", $"Bot Volume set to: {volume}");
                    return await EmbedHandler.BasicEmbed("", $"🔇 Громкость была установлена на {volume}. Теперь меня не слышно.", CreateConfig.Config.Tanya);
                }
                if (volume >= 1 && volume <= 20)
                {
                    await player.UpdateVolumeAsync((ushort)volume);
                    await LogService.LogInfoAsync("MUSIC", $"Bot Volume set to: {volume}");
                    return await EmbedHandler.BasicEmbed("", $"🔈 Громкость была установлена на {volume}.", CreateConfig.Config.Tanya);
                }
                if (volume >= 20 && volume <= 80)
                {
                    await player.UpdateVolumeAsync((ushort)volume);
                    await LogService.LogInfoAsync("MUSIC", $"Bot Volume set to: {volume}");
                    return await EmbedHandler.BasicEmbed("", $"🔉 Громкость была установлена на {volume}.", CreateConfig.Config.Tanya);
                }
                if (volume >= 80 && volume <= 150)
                {
                    await player.UpdateVolumeAsync((ushort)volume);
                    await LogService.LogInfoAsync("MUSIC", $"Bot Volume set to: {volume}");
                    return await EmbedHandler.BasicEmbed("", $"🔊 Громкость была установлена на {volume}.", CreateConfig.Config.Tanya);
                }

                return await EmbedHandler.BasicEmbed("", $"🔊 Громкость была установлена на {volume}.", CreateConfig.Config.Tanya);
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

            else if (loop is false && looplist is false)
            {
                args.Player.Queue.TryDequeue(out LavaTrack track);
                await args.Player.PlayAsync(track);
                await args.Player.TextChannel.SendMessageAsync(
                    embed: await EmbedHandler.BasicEmbed("🎵 Музыка", $"Сейчас Играет: \"{track.Title}\"\nДлительность: {track.Duration}\n Автор: {track.Author}\nUrl: {track.Url}", Color.Green));
            }
        }
    }
}
