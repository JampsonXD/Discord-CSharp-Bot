using Newtonsoft.Json;

namespace YoutubeClient.Models;

public enum YoutubeSearchType
{
    Channel,
    Playlist,
    Video
}

static class YoutubeSearchTypeExtensions
{
    public static string ToSearchParameterName(this YoutubeSearchType searchType)
    {
        switch (searchType)
        {
            case YoutubeSearchType.Channel:
                return "channel";
            case YoutubeSearchType.Playlist:
                return "playlist";
            case YoutubeSearchType.Video:
                return "video";
            
            // Default to video search
            default:
                return "video";
        }
    }
}

public enum YoutubeSearchResultsOrder
{
    Relevance,
    Date,
    Rating,
    Title,
    VideoCount,
    ViewCount
}

static class YoutubeSearchResultsOrderExtensions
{
    public static string ToSearchParameterName(this YoutubeSearchResultsOrder order)
    {
        switch (order)
        {
            case YoutubeSearchResultsOrder.Relevance:
                return "relevance";
            case YoutubeSearchResultsOrder.Date:
                return "date";
            case YoutubeSearchResultsOrder.Rating:
                return "rating";
            case YoutubeSearchResultsOrder.Title:
                return "title";
            case YoutubeSearchResultsOrder.VideoCount:
                return "videoCount";
            case YoutubeSearchResultsOrder.ViewCount:
                return "viewCount";
            
            // Default to relevance
            default:
                return "relevance";
        }
    }
}

public class YoutubeSearch
{
    // Type of content/response we contain
    [JsonProperty("kind")] 
    public required string Kind { get; set; }
    
    // The e tag of this resource
    [JsonProperty("etag")]
    public required string ETag { get; set; }
    
    // The id of the channel resource
    [JsonProperty("id")] 
    public required YoutubeSearchId Id { get; set; }
    
    [JsonProperty("snippet")]
    public YoutubeSearchSnippet? Snippet { get; set; }
    
}

public class YoutubeSearchId
{
    // Type of content/response we contain
    [JsonProperty("kind")] 
    public required string Kind { get; set; }
    
    // If the search result type is a youtube video, this will be set to the videos id
    [JsonProperty("videoId")] 
    public string? VideoId { get; set; }
    
    // If the search result type is a channel, this will be set to the channels id
    [JsonProperty("channelId")] 
    public string? ChannelId { get; set; }
    
    // If the search result type is a playlist, this will be set to the playlists id
    [JsonProperty("playlistId")] 
    public string? PlaylistId { get; set; }
}

public class YoutubeSearchSnippet
{
    // The time at which the search result resource was created
    [JsonProperty("publishedAt")] 
    public DateTime PublishedAt { get; set; }
    
    // The channel id that published the search result resource
    [JsonProperty("channelId")] 
    public required string ChannelId { get; set; }
    
    // Title of the search result
    [JsonProperty("title")] 
    public required string Title { get; set; }
    
    // Description of the search result
    [JsonProperty("description")] 
    public required string Description { get; set; }
    
    // Thumbnail information associated with the search result.
    [JsonProperty("thumbnails")] 
    public required YoutubeThumbnailData Thumbnails { get; set; }
    
    // Title of the channel that published the resource the search result identifies
    [JsonProperty("channelTitle")] 
    public required string ChannelTitle { get; set; }
    
    // Indicates whether a video or channel resource has live broadcast content. Valid values are upcoming/live/none
    [JsonProperty("liveBroadcastContent")] 
    public string? LiveBroadcastContent { get; set; }
}