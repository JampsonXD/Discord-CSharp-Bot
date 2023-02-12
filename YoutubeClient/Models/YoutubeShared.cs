using Newtonsoft.Json;

namespace YoutubeClient.Models;

public class YoutubeThumbnailData
{
    // The thumbnail's Url
    [JsonProperty("url")]
    public required string Url { get; set; }
    
    // Width of the thumbnail
    [JsonProperty("width")]
    public uint Width { get; set; }
    
    // Height of the thumbnail
    [JsonProperty("height")]
    public uint Height { get; set; }
}

public class YoutubeLocalizedData
{
    [JsonProperty("title")]
    public required string Title { get; set; }
    
    [JsonProperty("description")]
    public required string Description { get; set; }
}

/* Contains information about which countries content is allowed or blocked from.*/
public class YoutubeRegionRestrictionData
{
    /* A list of region codes that identify countries where the video is viewable.
     If this property is present and a country is not listed in its value, then the video is blocked from appearing in that country. 
     If this property is present and contains an empty list, the video is blocked in all countries.*/
    [JsonProperty("allowed")]
    public List<string>? Allowed { get; set; }
    
    /* A list of region codes that identify countries where the video is blocked.
     If this property is present and a country is not listed in its value, then the video is viewable in that country. 
     If this property is present and contains an empty list, the video is viewable in all countries.
     */
    [JsonProperty("blocked")]
    public List<string>? Blocked { get; set; }
}