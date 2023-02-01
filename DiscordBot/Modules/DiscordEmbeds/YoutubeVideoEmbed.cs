using Discord;
using YoutubeClient.Models;

namespace DiscordBot.Modules.DiscordEmbeds;

public class YoutubeVideoEmbed
{
    public static Embed CreateNewYoutubeVideoEmbed(YoutubePlaylistItem playlistItem)
    {
        YoutubePlaylistItemSnippet? snippet = playlistItem.Snippet;

        if (snippet != null)
        {
            return new EmbedBuilder().WithAuthor(snippet.VideoOwnerChannelTitle).WithTitle(snippet.Title)
                .WithImageUrl(snippet.Thumbnails["standard"].Url)
                .WithUrl($"https://www.youtube.com/watch?v={snippet.ResourceId.VideoId}")
                .WithColor(Color.Red)
                .WithCurrentTimestamp().Build();
        }

        throw new InvalidOperationException($"Youtube Playlist Item {playlistItem.ToString()} does not contain a valid Snippet!");
    }
}