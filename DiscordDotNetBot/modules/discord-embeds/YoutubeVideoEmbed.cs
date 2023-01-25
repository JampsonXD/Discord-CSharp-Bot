using Discord;
using YoutubeClient.youtube_api.models;

namespace Discord_CSharp_Bot.modules.discord_embeds;

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