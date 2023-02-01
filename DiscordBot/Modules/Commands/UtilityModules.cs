using Discord.Commands;

namespace DiscordBot.Modules.Commands
{
    public class UtilityModule : ModuleBase<SocketCommandContext>
    {
        private readonly IServiceProvider _services;

        public UtilityModule(IServiceProvider serviceProvider)
        {
            _services = serviceProvider;
        }
        
        [Command("ping")]
        [Summary("Tells the user what ping they have to the current server")]
        public async Task PingAsync()
        {
            await Context.Channel.SendMessageAsync($"Ping is currently {Context.Client.Latency}.");
        }
    }
}