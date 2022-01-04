namespace FinalMusicBot.Modules
{
    public class Help : InteractiveBase<SocketCommandContext>
    {
        private readonly IUtils _utils;
        public Help(IUtils utils)
        {
            _utils = utils;
        } 
        
        [Command("help")]
        private async Task help()
        {
            var embed = _utils.BuildEmbed(null, "Help:", "`%play` - plays audio \n `%join` - joins channel \n `%help` - displays help \n `%shuffle` - shuffles queue \n `%queue` - displays queue \n `%pause, %resume %skip` - U know what it does \n `%search` - searches for a music");
            await ReplyAsync(embed: embed);
        }
    }
}