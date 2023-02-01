using Discord;
using SpotifyClient.DataModels;

namespace DiscordBot.Modules.DiscordEmbeds;

public class SpotifyEmbeds
{
    public static Embed CreateNewSpotifyAlbumEmbed(SpotifyAlbum album)
    {
        return new EmbedBuilder().WithAuthor(album.Artists.FirstOrDefault()?.Name).WithTitle(album.Name)
                .WithImageUrl(album.Images.FirstOrDefault()?.Url)
                .WithUrl(album.SpotifyExternalUrls.Spotify)
                .WithColor(Color.Green)
                .WithCurrentTimestamp().Build();
    }
}