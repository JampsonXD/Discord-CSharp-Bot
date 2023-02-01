using Discord;
using Discord.WebSocket;

namespace DiscordBot.Modules.DiscordEmbeds;

public class GrammarCheckingEmbed
{
    public static Embed CreateGrammarCheckingEmbed(DiscordSocketClient client, string title, string description)
    {
        return new EmbedBuilder().WithAuthor(client.CurrentUser.Username).WithTitle(title).WithDescription(description)
            .WithCurrentTimestamp().Build();
    }
}