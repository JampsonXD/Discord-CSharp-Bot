using Newtonsoft.Json;

namespace YoutubeClient.Models;

// Represents a youtube channel and details that are given back from the api call
public class YoutubeChannel
{
    // Type of content/response we contain
    [JsonProperty("kind")] 
    public required string Kind { get; set; }
    
    // The e tag of this resource
    [JsonProperty("etag")]
    public required string ETag { get; set; }
    
    // The id of the channel resource
    [JsonProperty("id")] 
    public required string Id { get; set; }
    
    [JsonProperty("contentDetails")]
    public YoutubeChannelContentDetails? ContentDetails { get; set; }
    
}

public class YoutubeChannelContentDetails
{
    [JsonProperty("relatedPlaylists")]
    public required YoutubeChannelRelatedPlaylists RelatedPlaylists { get; set; }
}

public class YoutubeChannelRelatedPlaylists
{
    // Id for the liked videos of the related youtube channel
    [JsonProperty("likes")] 
    public string? LikedVideos { get; set; }
    
    // Id for the favorites of the related youtube channel
    [JsonProperty("favorites")]
    public string? FavoriteVideos { get; set; }
    
    // Id for the uploads playlist of the related youtube channel
    [JsonProperty("uploads")]
    public string? Uploads { get; set; }

}