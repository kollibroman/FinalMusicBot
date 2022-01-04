namespace FinalMusicBot
{
    public class Utils : IUtils
    {
        private readonly ILogger<Utils> _logger; //useful embeds to speed things up
        public Utils(ILogger<Utils> logger)
        {
            _logger = logger;
        }

        public Embed BuildEmbed(string Title, object content)
        {
            var builder = new EmbedBuilder()
                .AddField(Title, content)
                 .WithColor(new Color(255, 255, 255))
                .Build();
            return builder;
        }
        public Embed BuildEmbed(string title, object content, bool inline)
        {
            var builder = new EmbedBuilder()
                .WithColor(new Color(255, 255, 255))
                .AddField(title, content, inline)
                .Build();
            return builder;
        }

        public Embed BuildEmbed(SocketUser author, Color color, string title, object Content)
        {
            var builder = new EmbedBuilder()
                .WithAuthor(author)
                .WithColor(color)
                .AddField(title, Content)
                .Build();
            return builder;
        }
        public Embed BuildEmbed(SocketUser author, string title, object Content) //use in big amounts
        {
            var builder = new EmbedBuilder()
                .WithAuthor(author)
                .WithColor(new Color(255, 255, 255))
                .AddField(title, Content)
                .Build();
            return builder;
        }

        public Embed BuildEmbed(SocketUser author, Color color, string title, object content, EmbedFooterBuilder footer)
        {
            var builder = new EmbedBuilder()
                .WithAuthor(author)
                .WithColor(color)
                .AddField(title, content)
                .WithFooter(footer)
                .Build();
            return builder;
        }
    }
}