// See https://aka.ms/new-console-template for more information

using System.ComponentModel;
using ClientService.ClientService;
using ClientService.Core;
using ClientService.Core.Authorization;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Configuration;
using DiscordBot.Modules.TimedEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SaplingClient.Services;
using SpotifyClient.Authorization;
using SpotifyClient.Services;
using YoutubeClient.Services;
using RunMode = Discord.Commands.RunMode;

namespace DiscordBot
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
            var token = serviceProvider.GetRequiredService<IOptions<DiscordBotOptions>>().Value.Token;

            // Log our command services
            serviceProvider.GetRequiredService<CommandService>().Log += ApplicationLog;

            // Initialize our command handler and interaction handler
            await serviceProvider.GetRequiredService<CommandHandleService>().InitializeAsync();
            await serviceProvider.GetRequiredService<InteractionHandleService>().InitializeAsync();
            
            // As soon as our client is ready, register all interaction commands
            client.Ready += async () =>
            {
                await serviceProvider.GetRequiredService<InteractionService>().RegisterCommandsGloballyAsync();
            };

            // Login and start running our discord client
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            var taskHandler = serviceProvider.GetRequiredService<TimedTaskHandler>();

            // Check for mogul mail
            taskHandler.AddTask(new NewYoutubeVideoTimedTask(
                TimeSpan.FromMinutes(10),"UUjK0F1DopxQ5U0sCwOlXwOg", 183663101848059906, serviceProvider.GetRequiredService<YoutubeClientService>(), client));
            taskHandler.Start();
            
            await Task.Delay(-1);
        }

        private Task ApplicationLog(LogMessage arg)
        {
            Console.WriteLine($"{arg.Exception} - {arg.Severity}: {arg.Message}");
            return Task.CompletedTask;
        }

        private static ServiceProvider GetServiceProvider()
        {
            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false).Build();

            /* Add configuration options for each of our different services */
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddOptions<SpotifyAuthorizationOptions>()
                .Configure(config.GetSection(SpotifyAuthorizationOptions.Position).Bind);
            serviceCollection.AddOptions<YoutubeClientOptions>()
                .Configure(config.GetSection(YoutubeClientOptions.Position).Bind);
            serviceCollection.AddOptions<SaplingClientOptions>()
                .Configure(config.GetSection(SaplingClientOptions.Position).Bind);
            serviceCollection.AddOptions<DiscordBotOptions>()
                .Configure(config.GetSection(DiscordBotOptions.Position).Bind);

            /* Add all of our services */
            return serviceCollection
                .AddScoped<IConfiguration>(_ => config)
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    MessageCacheSize = 1000,
                    GatewayIntents = GatewayIntents.All
                }))
                .AddSingleton<InteractionService>(provider => new InteractionService(provider.GetRequiredService<DiscordSocketClient>(), new InteractionServiceConfig()
                {
                    DefaultRunMode = Discord.Interactions.RunMode.Async
                }))
                .AddSingleton<IContainer>(new Container())
                .AddSingleton<InteractionHandleService>()
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
                    ApiKey = provider.GetRequiredService<IOptions<YoutubeClientOptions>>().Value.ApiKey,
                    HttpClient = provider.GetRequiredService<HttpClient>()
                }))
                .AddSingleton<IAuthorizer, SpotifyAuthorizer>()
                .AddSingleton<OAuthHttpHandler>()
                .AddSingleton<SpotifyClientService>(provider => new SpotifyClientService(new ClientServiceInitializer()
                {
                    HttpClient = new HttpClient(provider.GetRequiredService<OAuthHttpHandler>())
                }))
                .AddSingleton<SaplingClientService>(provider => new SaplingClientService(new ClientServiceInitializer()
                {
                    ApiKey = provider.GetRequiredService<IOptions<SaplingClientOptions>>().Value.ApiKey,
                    HttpClient = provider.GetRequiredService<HttpClient>()
                }))
                .AddSingleton<CommandHandleService>()
                .AddScoped<Random>()
                .AddSingleton<TimedTaskHandler>()
                .BuildServiceProvider();
        }
    }
}