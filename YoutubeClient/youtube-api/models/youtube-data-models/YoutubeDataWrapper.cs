using Newtonsoft.Json;

namespace YoutubeClient.youtube_api.models.youtube_data_models;

// Contains information about the responses page, such as the total results found
public class YoutubeResponsePageInfo
{
    // Total number of results in a result set
    [JsonProperty("totalResults")]
    public int TotalResults { get; set; }
    
    // Number of results included in a response
    [JsonProperty("resultsPerPage")]
    public int ResultsPerPage { get; set; }
}

/* Wrapper class that holds response data for youtube api requests. Each response contains a "kind" of response (the type of content associated),
    a tag for the resource, page tokens for the next and previous page for a response, and response page data. The wrapper also contains a list of generic items that
    change based on the type of response.
 */
public class YoutubeResponseDataWrapper<T>
{
    internal YoutubeResponseDataWrapper() {}

    // Type of content/response we contain
    [JsonProperty("kind")] 
    public string Kind { get; set; }
    
    // The e tag of this resource (Useful for checking if a resource has changed or not)
    [JsonProperty("etag")]
    public string ETag { get; set; }
    
    // Token that can be used as a value of pageToken parameter to get the next page in a result set
    [JsonProperty("nextPageToken")]
    public string? NextPageToken { get; set; }
    
    // Token that can be used as a value of pageToken parameter to get the previous page in a result set
    [JsonProperty("prevPageToken")]
    public string? PreviousPageToken { get; set; }
    
    // Encapsulates paging info for a result set
    [JsonProperty("pageInfo")]
    public YoutubeResponsePageInfo PageInfo { get; set; }
    
    // list of resources that are returned from a response, this varies based on the response content type
    [JsonProperty("items")]
    public List<T> Items { get; set; }
    
}