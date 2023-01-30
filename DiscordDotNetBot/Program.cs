// See https://aka.ms/new-console-template for more information
using Discord;
using Discord_CSharp_Bot.modules.api;
using Discord_CSharp_Bot.modules.command_handling;
using Discord_CSharp_Bot.modules.timed_events;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YoutubeClient.youtube_api.services;

namespace Discord_CSharp_Bot
{
    public class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            ServiceProvider serviceProvider = GetServiceProvider();
            DiscordSocketClient client = serviceProvider.GetRequiredService<DiscordSocketClient>();
            client.Log += ApplicationLog;

            // Get the discord token from our config file
            var token = serviceProvider.GetRequiredService<IConfiguration>()["DiscordApiToken"];

            // Log our command services
            serviceProvider.GetRequiredService<CommandService>().Log += ApplicationLog;
            

            // Login and start running our discord client
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            
            // Initialize our command handler
            await serviceProvider.GetRequiredService<CommandHandleService>().InitializeAsync();

            var taskHandler = serviceProvider.GetRequiredService<TimedTaskHandler>();
            // Check for mogul mail
            taskHandler.AddTask(new NewYoutubeVideoTimedTask(
                TimeSpan.FromMinutes(10),"UUjK0F1DopxQ5U0sCwOlXwOg", 183663101848059906, serviceProvider.GetRequiredService<YoutubeClientService>(), client));
            taskHandler.Run();
            
            await Task.Delay(-1);
        }

        private Task ApplicationLog(LogMessage arg)
        {
            Console.WriteLine($"{arg.Exception} - {arg.Severity}: {arg.Message}");
            return Task.CompletedTask;
        }

        private static ServiceProvider GetServiceProvider()
        {
            IConfiguration config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false).Build();
            
            return new ServiceCollection()
                .AddScoped<IConfiguration>(_ => config)
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    MessageCacheSize = 1000,
                    GatewayIntents = GatewayIntents.All
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    CaseSensitiveCommands = false,
                    DefaultRunMode = RunMode.Async,
                    IgnoreExtraArgs = true,
                    LogLevel = LogSeverity.Verbose
                }))
                .AddSingleton<HttpClient>()
                .AddSingleton<YoutubeClientService>(provider => new YoutubeClientService(new ClientServiceInitializer()
                {
                    ApiKey = provider.GetRequiredService<IConfiguration>()["YoutubeApiKey"] ?? throw new InvalidOperationException(),
                    HttpClient = provider.GetRequiredService<HttpClient>()
                }))
                .AddSingleton<CommandHandleService>()
                .AddSingleton<SaplingApiClient>()
                .AddScoped<Random>()
                .AddScoped<IMemoryCache>(_ => new MemoryCache(new MemoryCacheOptions()))
                .AddSingleton<TimedTaskHandler>()
                .BuildServiceProvider();
        }
    }
}