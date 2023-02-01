using Newtonsoft.Json;

namespace SpotifyClient.DataModels;

public class AddedBy
{
    [JsonProperty("external_urls")]
    public SpotifyExternalUrls SpotifyExternalUrls { get; set; }

    [JsonProperty("href")]
    public Uri Href { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("uri")]
    public string Uri { get; set; }

    [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; set; }
}

public class ExternalIds
{
    [JsonProperty("isrc")]
    public string Isrc { get; set; }
}

public class VideoThumbnail
{
    [JsonProperty("url")]
    public object Url { get; set; }
}

public class SpotifyImage
{
    [JsonProperty("height")]
    public long? Height { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("width")]
    public long? Width { get; set; }
}