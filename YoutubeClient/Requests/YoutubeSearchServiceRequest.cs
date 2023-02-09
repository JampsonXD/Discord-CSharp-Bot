using ClientService.ServiceRequests;
using YoutubeClient.Models;

namespace YoutubeClient.Requests;

public class YoutubeSearchServiceRequest: YoutubeServiceRequest<YoutubeSearch>
{
    private int? _maxResults;
    public override string RelativePath => "search";
    
    /* Specifies search resource properties to include in the result. Search results only allow for snippet resources to be included */
    [RequestQueryParameter("part", true)] 
    public string Part => "snippet";

    /* Specifies that a search should only contain results from a specified channel */
    [RequestQueryParameter("channelId")]
    public string ChannelId { get; set; }
    
    /* Specifies the maximum amount of search results to be returned */
    [RequestQueryParameter("maxResults")]
    public int? MaxResults
    {
        get => _maxResults;
        set => _maxResults = Math.Clamp(value ?? 1, 1, 50);
    }
    
    /* Specifies the order in which search results should be returned */
    [RequestQueryParameter("order")]
    public string? Order { get; private set; }

    /* Query string to be sent with the request */
    [RequestQueryParameter("q", true)]
    public string Query { get; set; }
    
    /* Specifies the type of resource we want to search for (videos, channels, playlists) */
    [RequestQueryParameter("type")]
    public string? Type { get; private set; }
    
    public YoutubeSearchServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
    {
        
    }

    public void SetSearchResultOrder(YoutubeSearchResultsOrder order)
    {
        Order = order.ToSearchParameterName();
    }

    public void SetSearchType(YoutubeSearchType searchType)
    {
        Type = searchType.ToSearchParameterName();
    }

}