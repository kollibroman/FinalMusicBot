namespace FinalMusicBot
{
    public interface IUtils
    {
        public Embed BuildEmbed(string Title, object Content);
        public Embed BuildEmbed(string title, object content, bool inline);
        public Embed BuildEmbed(SocketUser author, Color color, string title, object Content);
        public Embed BuildEmbed(SocketUser author, string title, object Content);
        public Embed BuildEmbed(SocketUser author, Color color, string title, object content, EmbedFooterBuilder footer);
    }
}