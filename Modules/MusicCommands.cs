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
            var player = _node.GetPlayer(Context.Guild);
            if(player.VoiceChannel == null)
            {
                var embed = _utils.BuildEmbed("**No No No**", "U aren't in the voice channel");
                await ReplyAsync(embed: embed);
                return;
            }

            await _node.ConnectAsync();
            var e = _utils.BuildEmbed("**Uh Oh**", $"You connected me to {Context.Channel}");
            await ReplyAsync(embed: e);
        }
    }
}