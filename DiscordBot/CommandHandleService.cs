using System.Diagnostics;
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Configuration;
using DiscordBot.Core;
using DiscordBot.Modules.DiscordEmbeds;
using DiscordBot.Modules.Sapling;
using DiscordBot.Modules.TimedEvents;
using Microsoft.Extensions.Options;
using SaplingClient.Exceptions;
using SaplingClient.Services;

namespace DiscordBot
{
    public class CommandHandleService
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly CommandService _commands;
        private readonly SaplingClientService _saplingClient;
        private readonly DiscordBotOptions _options;
        private readonly Random _random;

        public CommandHandleService(CommandService commands, DiscordSocketClient client, 
            IServiceProvider services, SaplingClientService saplingClient, IOptions<DiscordBotOptions> options, Random random, ISubscriptionService subscriptionService)
        {
            _commands = commands;
            _client = client;
            _services = services;
            _saplingClient = saplingClient;
            _options = options.Value;
            _random = random;
            
            _client.MessageReceived += ClientOnMessageReceived;
            _client.Ready += ClientOnReady;
            subscriptionService.Subscribe<TimedTaskMessage>(OnTimeTaskMessageReceived);
        }

        private void OnTimeTaskMessageReceived(TimedTaskMessage message)
        {
            Task.Run(() => NotifyOfTimeTaskMessageReceived(message));
        }

        private async Task NotifyOfTimeTaskMessageReceived(TimedTaskMessage message)
        {
            var user = _client.GetUser(message.TimedTaskInformation.DiscordUserId);
            var action = message.Added ? "Added" : "Removed";
            await _client.GetUser(183663101848059906).SendMessageAsync(
                $"Time task has been {action} for discord user {user.Username} following channel id {message.TimedTaskInformation.ChannelId}");
        }
        

        private async Task ClientOnReady()
        {
            await SetDiscordBotStatus();
        }

        private async Task SetDiscordBotStatus()
        {
            await _client.SetStatusAsync(UserStatus.Online);
            await _client.SetGameAsync(_options.GameStatus, "", ActivityType.Playing);
        }

        private async Task ClientOnMessageReceived(SocketMessage messageParam)
        {
            if (messageParam.Author.IsBot || !(messageParam is SocketUserMessage message))
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);
            
            int argPos = 0;
            if (!message.HasStringPrefix(_options.Prefix, ref argPos))
            {
                // Check first if the message needs to be correct and then return early
                await HandleGrammarChecking(message);
                return;
            }
            
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        private async Task HandleGrammarChecking(SocketUserMessage socketUserMessage)
        {
            // Check if we have any id's to handle grammar correction
            var idList = _options.GrammarBotSettings.GrammarCheckIds;
            if (idList != null)
            {
                // If we find any id's that match our authors id, perform grammar and spelling checking
                if (idList.Any(x => x == socketUserMessage.Author.Id))
                {
                    await ValidateUserMessage(socketUserMessage);
                }
            }
        }

        private async Task ValidateUserMessage(SocketUserMessage socketUserMessage)
        {
            try
            {
                var request = _saplingClient.GrammarCorrectionResource.GrammarCheckingService();
                request.Message = socketUserMessage.Content;
                var response = await request.ConvertSaplingRequestIntoResponseData();

                // Only alert users if our message was modified
                if (response.IsContentModified)
                {
                    string preMessage = _options.GrammarBotSettings.DefaultGrammarResponse;
                
                    {
                        var messages = _options.GrammarBotSettings.GrammarCorrectionResponses;
                        if (messages is {Count: > 0})
                        {
                            preMessage = messages[_random.Next(0, messages.Count)];
                        }
                    }
                
                    Debug.Assert(response.ModifiedMessage != null, "response.ModifiedMessage != null");
                    // Create our embedded message
                    Embed embed =
                        GrammarCheckingEmbed.CreateGrammarCheckingEmbed(_client, preMessage, response.ModifiedMessage);
                    await socketUserMessage.Channel.SendMessageAsync(null, false, embed);
                }
            }
            catch (SaplingInvalidResponseException responseException)
            {
                Console.WriteLine($"Error in requesting Sapling Response Data. \n Error: {responseException.Message}");
            }
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
    }
}