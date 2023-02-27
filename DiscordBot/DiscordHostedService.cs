using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Configuration;
using DiscordBot.Core;
using DiscordBot.Modules.TimedEvents;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using YoutubeClient.Services;

namespace DiscordBot;

public class DiscordHostedService: IHostedService
{
    private readonly DiscordSocketClient _discordClient;
    private readonly DiscordBotOptions _options;
    private readonly CommandService _commandService;
    private readonly InteractionService _interactionService;
    private readonly CommandHandleService _commandHandleService;
    private readonly InteractionHandleService _interactionHandleService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly YoutubeClientService _youtubeClientService;
    private readonly TimedTaskHandler _timedTaskHandler;

    public DiscordHostedService(DiscordSocketClient discordClient, IOptions<DiscordBotOptions> options,
        CommandService commandService, InteractionService interactionService, CommandHandleService commandHandleService, InteractionHandleService interactionHandleService, ISubscriptionService subscriptionService, YoutubeClientService youtubeClientService, TimedTaskHandler timedTaskHandler)
    {
        _discordClient = discordClient;
        _options = options.Value;
        _commandService = commandService;
        _interactionService = interactionService;
        _commandHandleService = commandHandleService;
        _interactionHandleService = interactionHandleService;
        _subscriptionService = subscriptionService;
        _youtubeClientService = youtubeClientService;
        _timedTaskHandler = timedTaskHandler;
    }

    private async Task Setup()
    {
        _discordClient.Log += ApplicationLog;

        // Log our command services
        _commandService.Log += ApplicationLog;

        // Initialize our command handler and interaction handler
        await _commandHandleService.InitializeAsync();
        await _interactionHandleService.InitializeAsync();
            
        // As soon as our client is ready, register all interaction commands
        _discordClient.Ready += async () =>
        {
            await _interactionService.RegisterCommandsGloballyAsync();
        };

        _subscriptionService.Subscribe<YoutubeTimedTaskDatabaseUpdateMessage>(
            OnYoutubeTimedTaskDatabaseUpdateMessageReceived);

        // Login and start running our discord client
        await _discordClient.LoginAsync(TokenType.Bot, _options.Token);
        await _discordClient.StartAsync();
    }

    private void OnYoutubeTimedTaskDatabaseUpdateMessageReceived(YoutubeTimedTaskDatabaseUpdateMessage message)
    {
        if (message.Added)
        {
            _timedTaskHandler.AddTask(message.ToTimedTask(_youtubeClientService, _discordClient));
        }
        else
        {
            if (_timedTaskHandler.FindTask(message.MatchesTimedTask, out var foundTask) && foundTask != null)
            {
                _timedTaskHandler.RemoveTask(foundTask);
            }
        }
    }

    private Task ApplicationLog(LogMessage arg)
    {
        Console.WriteLine($"{arg.Exception} - {arg.Severity}: {arg.Message}");
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.Run(Setup, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Shutting down Discord Hosted Service");
        return Task.CompletedTask;
    }
}