using Discord;
using Discord.WebSocket;

namespace Discord_CSharp_Bot.modules.discord_embeds;

public class GrammarCheckingEmbed
{
    public static Embed CreateGrammarCheckingEmbed(DiscordSocketClient client, string title, string description)
    {
        return new EmbedBuilder().WithAuthor(client.CurrentUser.Username).WithTitle(title).WithDescription(description)
            .WithCurrentTimestamp().Build();
    }
}