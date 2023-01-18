using Newtonsoft.Json;

namespace Discord_CSharp_Bot.modules.youtube_api.models;

// Represents a youtube channel and details that are given back from the api call
public class YoutubeChannel
{
    internal YoutubeChannel() {}
    
    // Type of content/response we contain
    [JsonProperty("kind")] 
    public string Kind { get; set; }
    
    // The e tag of this resource
    [JsonProperty("etag")]
    public string ETag { get; set; }
    
    // The id of the channel resource
    [JsonProperty("id")] 
    public string Id { get; set; }
    
    [JsonProperty("contentDetails")]
    public YoutubeChannelContentDetails? ContentDetails { get; set; }
    
}

public class YoutubeChannelContentDetails
{
    internal YoutubeChannelContentDetails() {}

    [JsonProperty("relatedPlaylists")]
    public YoutubeChannelRelatedPlaylists RelatedPlaylists { get; set; }
}

public class YoutubeChannelRelatedPlaylists
{
    internal YoutubeChannelRelatedPlaylists() {}

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