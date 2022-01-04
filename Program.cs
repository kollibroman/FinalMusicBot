namespace FinalMusicBot
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();

            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration(x => 
                {
                    x.AddConfiguration(builder);
                })
                .ConfigureLogging(x =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(builder)
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .CreateLogger();
                    x.AddSerilog(Log.Logger);
                })
                .ConfigureDiscordHost((context, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = Discord.LogSeverity.Debug,
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200
                    };
                    config.Token = context.Configuration["token"];
                })
                .UseCommandService((context, config) =>
                {
                    config.DefaultRunMode = RunMode.Async;
                    config.CaseSensitiveCommands = false;
                })
                .UseInteractionService((context, config) => 
                {
                    config.LogLevel = Discord.LogSeverity.Debug;
                    config.UseCompiledLambda = true;
                })
                .ConfigureServices(services => 
                {
                    services.AddHostedService<CommandHandler>();
                    services.AddLavaNode(x =>
                    {
                        x.SelfDeaf = false;
                        x.LogSeverity = Discord.LogSeverity.Debug;
                    });
                    services.AddSingleton<InteractiveService>();
                })
                .UseSerilog()
                .UseConsoleLifetime();

            using(var host = hostBuilder.Build())
            {
                await host.RunAsync();
            }
        }
    }
}
