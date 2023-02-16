using ClientService.Core.Authorization;

namespace SpotifyClient.Authorization;

public class SpotifyAuthorizationOptions: IAuthorizationConfigurationOptions
{
    public const string Position = "Spotify";
    public Uri TokenRequestUri => new("https://accounts.spotify.com/api/token");
    public required string ClientSecret { get; set; }
    public required string ClientId { get; set; }
}