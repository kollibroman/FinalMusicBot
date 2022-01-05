namespace FinalMusicBot.Services
{
    public class CommandHandler : IHostedService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly IConfiguration _config;
        private readonly LavaNode _node;
        public CommandHandler(IConfiguration config, LavaNode node, CommandService service, DiscordSocketClient client, IServiceProvider provider)
        {
            _provider = provider;
            _client = client;
            _service = service;
            _config = config;
            _node = node;
        }

        public async Task StartAsync(CancellationToken cancellationtoken)
        {
            _client.Ready += OnReady;
            _client.MessageReceived += OnMessageReceived;
            _service.CommandExecuted += OnCommandExecuted;
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
        public async Task StopAsync(CancellationToken token)
        {
           await Task.CompletedTask;
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasStringPrefix(_config["prefix"], ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _service.ExecuteAsync(context, argPos, _provider);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            var embed = new EmbedBuilder()
                .WithColor(new Color(255, 255, 255))
                .AddField("This is not how ya dew it broo!", result)
                .Build();

            if (command.IsSpecified && !result.IsSuccess) await context.Channel.SendMessageAsync(embed: embed);
        }

        private async Task OnReady()
        {
            if (!_node.IsConnected)
            {
                await _node.ConnectAsync();
            }
        }
    }
}