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
using DiscordBot.Configuration.Validation;
using DiscordBot.Core;
using DiscordBot.Modules.TimedEvents;
using DiscordBot.Modules.TimedEvents.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            var builder = new HostBuilder();
            var host = builder.ConfigureAppConfiguration(ConfigureAppConfiguration)
                .ConfigureServices(ConfigureAppServices)
                .Build();
            await host.RunAsync();
        }

        private void ConfigureAppConfiguration(HostBuilderContext context, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false);
        }

        private void ConfigureAppServices(HostBuilderContext context, IServiceCollection serviceCollection)
        {
            serviceCollection.AddValidatorsFromAssemblyContaining<Program>();

            serviceCollection.AddOptions<SpotifyAuthorizationOptions>()
                .Bind(context.Configuration.GetSection(SpotifyAuthorizationOptions.Position));
            serviceCollection.AddOptions<YoutubeClientOptions>()
                .Bind(context.Configuration.GetSection(YoutubeClientOptions.Position));
            serviceCollection.AddOptions<SaplingClientOptions>()
                .Bind(context.Configuration.GetSection(SaplingClientOptions.Position));
            serviceCollection.AddOptions<DiscordBotOptions>()
                .Bind(context.Configuration.GetSection(DiscordBotOptions.Position))
                .ValidateFluently()
                .ValidateOnStart();

            /* Add Database Context */
            serviceCollection.AddDbContext<DiscordBotDatabase>(options =>
            {
                options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"));
            });
            
            /* Add all of our services */
            serviceCollection
                .AddSingleton<IConfiguration>(_ => context.Configuration)
                .AddSingleton<MessageProcessor>()
                .AddHostedService<MessageProcessor>(provider => provider.GetRequiredService<MessageProcessor>())
                .AddSingleton<IMessageSink>(provider => provider.GetRequiredService<MessageProcessor>())
                .AddSingleton<ISubscriptionService>(provider => provider.GetRequiredService<MessageProcessor>())
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    MessageCacheSize = 1000,
                    GatewayIntents = GatewayIntents.All
                }))
                .AddSingleton<InteractionService>(provider => new InteractionService(
                    provider.GetRequiredService<DiscordSocketClient>(), new InteractionServiceConfig()
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
                .AddHostedService<DiscordHostedService>()
                .AddScoped<YoutubeTimedTaskDatabaseRetriever>()
                .AddSingleton<TimedTaskHandler>(provider =>
                {
                    List<ITimedTask> list;
                    using (var scope = provider.CreateScope())
                    {
                        list = scope.ServiceProvider.GetRequiredService<YoutubeTimedTaskDatabaseRetriever>().GetTimedTasks().ToList();
                    }
                    return new TimedTaskHandler(list, provider);
                })
                .AddHostedService<TimedTaskHandler>(provider => provider.GetRequiredService<TimedTaskHandler>());
        }
    }
}