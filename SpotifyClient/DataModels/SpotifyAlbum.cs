using Newtonsoft.Json;

namespace SpotifyClient.DataModels;

public class SpotifyAlbum
{
    [JsonProperty("album_type")]
    public string AlbumType { get; set; }

    [JsonProperty("artists")]
    public AddedBy[] Artists { get; set; }

    [JsonProperty("available_markets")]
    public string[] AvailableMarkets { get; set; }

    [JsonProperty("external_urls")]
    public SpotifyExternalUrls SpotifyExternalUrls { get; set; }

    [JsonProperty("href")]
    public string Href { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("images")]
    public SpotifyImage[] Images { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("release_date")]
    public string ReleaseDate { get; set; }

    [JsonProperty("release_date_precision")]
    public string ReleaseDatePrecision { get; set; }

    [JsonProperty("total_tracks")]
    public long TotalTracks { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("uri")]
    public string Uri { get; set; }
}