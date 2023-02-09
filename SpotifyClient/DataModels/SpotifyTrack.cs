using Newtonsoft.Json;

namespace SpotifyClient.DataModels;

public class SpotifyTrack
{
    [JsonProperty("album")]
    public SpotifyAlbum SpotifyAlbum { get; set; }

    [JsonProperty("artists")]
    public AddedBy[] Artists { get; set; }

    [JsonProperty("available_markets")]
    public string[] AvailableMarkets { get; set; }

    [JsonProperty("disc_number")]
    public long DiscNumber { get; set; }

    [JsonProperty("duration_ms")]
    public long DurationMs { get; set; }

    [JsonProperty("episode")]
    public bool Episode { get; set; }

    [JsonProperty("explicit")]
    public bool Explicit { get; set; }

    [JsonProperty("external_ids")]
    public ExternalIds ExternalIds { get; set; }

    [JsonProperty("external_urls")]
    public SpotifyExternalUrls SpotifyExternalUrls { get; set; }

    [JsonProperty("href")]
    public Uri Href { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("is_local")]
    public bool IsLocal { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("popularity")]
    public long Popularity { get; set; }

    [JsonProperty("preview_url")]
    public Uri PreviewUrl { get; set; }

    [JsonProperty("track")]
    public bool TrackTrack { get; set; }

    [JsonProperty("track_number")]
    public long TrackNumber { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("uri")]
    public string Uri { get; set; }
}