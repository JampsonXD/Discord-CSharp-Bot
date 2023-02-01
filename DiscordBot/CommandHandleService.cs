using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Modules.DiscordEmbeds;
using DiscordBot.Modules.Sapling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordBot
{
    public class CommandHandleService
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly CommandService _commands;
        
        public CommandHandleService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            
            _client.MessageReceived += ClientOnMessageReceived;
            _client.Ready += ClientOnReady;
        }

        private async Task ClientOnReady()
        {
            await SetDiscordBotStatus();
        }

        private async Task SetDiscordBotStatus()
        {
            await _client.SetStatusAsync(UserStatus.Online);
            await _client.SetGameAsync("One of the Paragon Remakes", "", ActivityType.Playing);
        }

        private async Task ClientOnMessageReceived(SocketMessage messageParam)
        {
            if (messageParam.Author.IsBot || !(messageParam is SocketUserMessage message))
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);
            
            int argPos = 0;
            string? prefix = _services.GetRequiredService<IConfiguration>()["DiscordPrefix"];
            if (prefix != null && !message.HasStringPrefix(prefix, ref argPos))
            {
                // Check first if the message needs to be correct and then return early
                await HandleGrammarChecking(message);
                return;
            }
            
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        private async Task HandleGrammarChecking(SocketUserMessage socketUserMessage)
        {
            // Get a list of id's we have in our app configuration for grammar checking
            var idList = _services.GetRequiredService<IConfiguration>().GetSection("grammar-check").Get<List<ulong>>();
            if (idList != null)
            {
                // If we find any id's that match our authors id, perform grammar and spelling checking
                if (idList.Any(x => x == socketUserMessage.Author.Id))
                {
                    ValidateUserMessage(socketUserMessage);
                }
            }
        }

        private async Task ValidateUserMessage(SocketUserMessage socketUserMessage)
        {
            var response = await _services.GetRequiredService<SaplingApiClient>()
                .RequestGrammarCorrectionAsync(socketUserMessage.Content);

            // Only alert users if our message was modified
            if (response.IsContentModified)
            {
                List<string>? grammarPreMessages = _services.GetRequiredService<IConfiguration>().GetSection("bot-pre-message")
                    .Get<List<string>>();
                string preMessage = "GRAMMAR ALERT!";
                if (grammarPreMessages is {Count: > 0})
                {
                    var rand = _services.GetRequiredService<Random>();
                    preMessage = grammarPreMessages[rand.Next(0, grammarPreMessages.Count)];
                }

                // Create our embedded message
                Embed embed =
                    GrammarCheckingEmbed.CreateGrammarCheckingEmbed(_client, preMessage, response.ModifiedMessage);
                await socketUserMessage.Channel.SendMessageAsync(null, false, embed);
            }
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
    }
}