using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace DiscordBot;

public class InteractionHandleService
{
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly DiscordSocketClient _discordClient;

    public InteractionHandleService(InteractionService interactionService, DiscordSocketClient discordClient, IServiceProvider services)
    {
        _interactionService = interactionService;
        _serviceProvider = services;
        _discordClient = discordClient;
    }

    public async Task InitializeAsync()
    {
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        _discordClient.InteractionCreated += DiscordClientOnInteractionCreated;
    }

    private async Task DiscordClientOnInteractionCreated(SocketInteraction arg)
    {
        try
        {
            var context = new SocketInteractionContext(_discordClient, arg);
            await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
    
}