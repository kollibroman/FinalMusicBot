namespace FinalMusicBot.Modules
{
    public class MusicCommands : InteractiveBase<SocketCommandContext>
    {
        private readonly LavaNode _node;
        private readonly IUtils _utils;
        public MusicCommands(LavaNode node, IUtils utils)
        {
            _node = node;
            _utils = utils;
        }

        [Command("join")]
        private async Task Join()
        {
            if(_node.HasPlayer(Context.Guild))
            {
                var emb = _utils.BuildEmbed("No No No", "Me already connected");
                await ReplyAsync(embed: emb);
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if(voiceState?.VoiceChannel == null)
            {
                var e = _utils.BuildEmbed("No No No", "Connect to the vc channel first");
                await ReplyAsync(embed: e);
                return;
            }

            try
            {
                await _node.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                var builder = _utils.BuildEmbed("Uh Oh", $"Am in {voiceState.VoiceChannel.Name}");
                await ReplyAsync(embed: builder);
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("play", RunMode = RunMode.Async), Alias("p")]
        public async Task PlayAsync([Remainder] string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                var embed = _utils.BuildEmbed("Gib me da title retard!", null);
                await ReplyAsync(embed: embed);
                return;
            }

            if (!_node.HasPlayer(Context.Guild))
            {
                var embed = _utils.BuildEmbed("LET ME IN DA VOICE CHANNEL YUO MORON!", null);
                await ReplyAsync(embed: embed);
                return;
            }

            var queries = searchQuery.Split(' ');
            foreach (var query in queries)
            {
                var searchResponse = await _node.SearchAsync(SearchType.YouTubeMusic, query);

                if (query.Contains("https"))
                {
                    searchResponse = await _node.SearchAsync(SearchType.Direct, query);
                }

                else
                {
                    searchResponse = await _node.SearchYouTubeAsync(query);
                }


                if (searchResponse.Status == SearchStatus.LoadFailed ||
                    searchResponse.Status == SearchStatus.NoMatches)
                {
                    var em = _utils.BuildEmbed("Sumimasen master pero no consejos encotro", query, true);
                    await ReplyAsync(embed: em);
                    return;
                }

                var player = _node.GetPlayer(Context.Guild);

                if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
                {
                    if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name) && query.Contains("https"))
                    {
                        foreach (var track in searchResponse.Tracks)
                        {
                            player.Queue.Enqueue(track);
                        }

                        var emb = _utils.BuildEmbed("Enqueued:", $"{searchResponse.Tracks.Count} tracks", true);
                        await ReplyAsync(embed: emb);
                    }
                    else
                    {
                        var track = searchResponse.Tracks.ElementAt(0);
                        player.Queue.Enqueue(track);
                        var embeded = _utils.BuildEmbed("Enqueued:", track.Title, true);
                        await ReplyAsync(embed: embeded);
                    }
                }
                else
                {
                    var track = searchResponse.Tracks.ElementAt(0);

                    if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name) && query.Contains("https"))
                    {
                        for (var i = 0; i < searchResponse.Tracks.Count; i++)
                        {
                            if (i == 0)
                            {
                                await player.PlayAsync(track);

                                var Embed = _utils.BuildEmbed("Now Playing", track.Title);
                                await ReplyAsync(embed: Embed);
                            }
                            else
                            {
                                player.Queue.Enqueue(searchResponse.Tracks.ElementAt(i));
                            }
                        }

                        var embed = _utils.BuildEmbed($"Enqueued {searchResponse.Tracks.Count} tracks", null);
                        await ReplyAsync(embed: embed);
                    }
                    else
                    {
                        await player.PlayAsync(track);
                        var eb = _utils.BuildEmbed("**Now playing:**", track.Title, true);
                        await ReplyAsync(embed: eb);
                    }
                }
            }
            _node.OnTrackEnded += OnTrackEnded;
            _node.OnTrackStuck += OnTrackStuck;
            _node.OnTrackException += OnTrackException;
        }

        [Command("leave"), Alias("fuckoff")]
        private async Task Leave()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                var e = _utils.BuildEmbed("No No No", "Connect to the vc channel first");
                await ReplyAsync(embed: e);
                return;
            }

            try
            {
                await _node.LeaveAsync(voiceState.VoiceChannel);
                var builder = _utils.BuildEmbed("Uh Oh", $"Disconnected :3");
                await ReplyAsync(embed: builder);
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("skip")]
        private async Task Skip()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                var e = _utils.BuildEmbed("No No No", "Connect to the vc channel first");
                await ReplyAsync(embed: e);
                return;
            }

            try
            {
                var player = _node.GetPlayer(Context.Guild);
                await player.SkipAsync();
                var builder = _utils.BuildEmbed("**Skipping**", $"Skipped :3");
                await ReplyAsync(embed: builder);
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        //Events
        private async Task OnTrackEnded(TrackEndedEventArgs args)
        {
            if(args.Reason.ShouldPlayNext())
            {
                return;
            }
            var player = args.Player;
            if (!player.Queue.TryDequeue(out var queueable))
            {
                var e = _utils.BuildEmbed("Queue Completed", "Gib more tracks");
                await player.TextChannel.SendMessageAsync(embed: e);
                return;
            }

            if (!(queueable is LavaTrack track))
            {
                var emb = _utils.BuildEmbed("Big nono", "Next item in queue is not a track");
                await player.TextChannel.SendMessageAsync(embed: emb);
                return;
            }

            await args.Player.PlayAsync(track);
            var embed = _utils.BuildEmbed("YES", $"now playing: {track.Title}");
            await player.TextChannel.SendMessageAsync(embed: embed);
        }

        private async Task OnTrackStuck(TrackStuckEventArgs args)
        {
            var player = args.Player;

            var embed = _utils.BuildEmbed("Error:", "Track got stuck, call step-player to unstuck it");
            await player.TextChannel.SendMessageAsync(embed: embed);
            await args.Player.SkipAsync();
        }

        private async Task OnTrackException(TrackExceptionEventArgs args)
        {
            var player = args.Player;

            var embed = _utils.BuildEmbed("Error:", $"{args.Exception.Message}");
            await player.TextChannel.SendMessageAsync(embed: embed);
            await args.Player.SkipAsync();
        }
    }
}