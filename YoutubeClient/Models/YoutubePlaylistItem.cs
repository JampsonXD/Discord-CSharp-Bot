﻿using Newtonsoft.Json;

namespace YoutubeClient.Models;

public class YoutubePlaylistItem
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

    [JsonProperty("snippet")] 
    public YoutubePlaylistItemSnippet? Snippet { get; set; }
}

/* Contains basic details about the playlist item, such as its title and position in the playlist */
public class YoutubePlaylistItemSnippet
{
    [JsonProperty("publishedAt")]
    public DateTime PublishedAt { get; set; }
    
    [JsonProperty("channelId")]
    public required string ChannelId { get; set; }
    
    [JsonProperty("title")]
    public required string Title { get; set; }
    
    [JsonProperty("description")]
    public required string Description { get; set; }
    
    /* A map of thumbnail images associated with the playlist item.
     For each object in the map, the key is the name of the thumbnail image, 
     and the value is an object that contains other information about the thumbnail.
     
     Valid key values are:
    default – The default thumbnail image. The default thumbnail for a video – or a resource that refers to a video, 
    such as a playlist item or search result – is 120px wide and 90px tall. The default thumbnail for a channel is 88px wide and 88px tall.
    
    medium – A higher resolution version of the thumbnail image. For a video (or a resource that refers to a video), 
    this image is 320px wide and 180px tall. For a channel, this image is 240px wide and 240px tall.
    
    high – A high resolution version of the thumbnail image. For a video (or a resource that refers to a video), this image is 480px wide and 360px tall. 
    For a channel, this image is 800px wide and 800px tall.
    
    standard – An even higher resolution version of the thumbnail image than the high resolution image. 
    This image is available for some videos and other resources that refer to videos, like playlist items or search results. 
    This image is 640px wide and 480px tall.
    
    maxres – The highest resolution version of the thumbnail image. This image size is available for some videos and other resources that refer to videos, 
    like playlist items or search results. This image is 1280px wide and 720px tall.
     */
    
    [JsonProperty("thumbnails")] 
    public required Dictionary<string, YoutubeThumbnailData> Thumbnails { get; set; }
    
    // Channel title that the playlist item belongs to
    [JsonProperty("channelTitle")]
    public required string ChannelTitle { get; set; }
    
    // The channel title of the channel that uploaded the video
    [JsonProperty("videoOwnerChannelTitle")]
    public required string VideoOwnerChannelTitle { get; set; }
    
    // The channel Id of the channel that uploaded this video
    [JsonProperty("videoOwnerChannelId")]
    public required string VideoOwnerChannelId { get; set; }
    
    // The Id that youtube uses to uniquely identify the playlist that the playlist item is in
    [JsonProperty("playlistId")]
    public required string PlayListId { get; set; }
    
    // The order in which the item appears in the playlist. Value uses a zero based index, so the first item has a position of 0, the second item has a position of 1, and so forth.
    [JsonProperty("position")]
    public uint Position { get; set; }

    [JsonProperty("resourceId")]
    public required YoutubePlaylistItemSnippetResourceId ResourceId { get; set; }
}

public class YoutubePlaylistItemSnippetResourceId
{
    // Kind/Type of referred resource
    [JsonProperty("kind")]
    public required string Kind { get; set; }
        
    // If this resource is a youtube#video, this property will be present and its value will contain the Id that is used to identify the video in the playlist
    [JsonProperty("videoId")]
    public string? VideoId { get; set; }
}