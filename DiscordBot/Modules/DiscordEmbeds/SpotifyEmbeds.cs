using Discord;
using SpotifyClient.DataModels;

namespace DiscordBot.Modules.DiscordEmbeds;

public static class SpotifyEmbeds
{
    public static Embed CreateNewSpotifyAlbumEmbed(SpotifyAlbum album)
    {
        return new EmbedBuilder().WithAuthor(album.Artists.FirstOrDefault()?.Name).WithTitle(album.Name)
                .WithImageUrl(album.Images.FirstOrDefault()?.Url)
                .WithUrl(album.SpotifyExternalUrls.Spotify)
                .WithColor(Color.Green)
                .WithCurrentTimestamp().Build();
    }

    public static Embed CreateNewSpotifyTrackEmbed(SpotifyTrack track)
    {
        return new EmbedBuilder().WithAuthor(track.Artists.FirstOrDefault()?.Name).WithTitle(track.Name)
            .WithImageUrl(track.SpotifyAlbum.Images.FirstOrDefault()?.Url)
            .WithUrl(track.SpotifyExternalUrls.Spotify)
            .WithColor(Color.Green)
            .WithCurrentTimestamp().Build();
    }
}