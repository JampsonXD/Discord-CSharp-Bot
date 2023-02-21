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
    private readonly DiscordBotDatabase _database;
    private readonly TimedTaskHandler _handler;
    private readonly ISubscriptionService _subscriptionService;
    private readonly YoutubeClientService _youtubeClient;

    public DiscordHostedService(DiscordSocketClient discordClient, IOptions<DiscordBotOptions> options,
        CommandService commandService, InteractionService interactionService, CommandHandleService commandHandleService, InteractionHandleService interactionHandleService, TimedTaskHandler handler, DiscordBotDatabase database, ISubscriptionService subscriptionService, YoutubeClientService youtubeClient)
    {
        _discordClient = discordClient;
        _options = options.Value;
        _commandService = commandService;
        _interactionService = interactionService;
        _commandHandleService = commandHandleService;
        _interactionHandleService = interactionHandleService;
        _handler = handler;
        _database = database;
        _subscriptionService = subscriptionService;
        _youtubeClient = youtubeClient;
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

        // Login and start running our discord client
        await _discordClient.LoginAsync(TokenType.Bot, _options.Token);
        await _discordClient.StartAsync();

        // Create new Timed Tasks from our Timed Task Information stored in our database
        _handler.AddTasks(_database.YoutubeTimedTaskInformation.Select(info => 
            new NewYoutubeVideoTimedTask(info.Interval, info.ChannelId, info.DiscordUserId, _youtubeClient, _discordClient)));

        // Notify our Timed Task Handler handling objects when a youtube timed task has been added or removed from the database
        _subscriptionService.Subscribe<TimedTaskMessage>(OnTimedTaskMessageReceived);
        _handler.Start();
    }

    private Task ApplicationLog(LogMessage arg)
    {
        Console.WriteLine($"{arg.Exception} - {arg.Severity}: {arg.Message}");
        return Task.CompletedTask;
    }

    private void OnTimedTaskMessageReceived(TimedTaskMessage message)
    {
        _handler.NotifyTimedTaskHandler(message, _youtubeClient, _discordClient);
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