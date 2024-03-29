﻿using ClientService.ClientService;
using ClientService.Core.Validation;
using ClientService.ServiceRequests;
using YoutubeClient.Models;

namespace YoutubeClient.Requests;

public sealed class YoutubeSearchServiceRequest: YoutubeServiceRequest<YoutubeSearch>
{
    private int? _maxResults;
    public override HttpMethod HttpMethod => HttpMethod.Get;
    public override string RelativePath => "search";
    
    /* Specifies search resource properties to include in the result. Search results only allow for snippet resources to be included */
    [QueryParameter("part")] 
    [ValidatePropertyNotNull]
    public string Part => "snippet";

    /* Specifies that a search should only contain results from a specified channel */
    [QueryParameter("channelId")]
    public string? ChannelId { get; set; }
    
    /* Specifies the maximum amount of search results to be returned */
    [QueryParameter("maxResults")]
    public int? MaxResults
    {
        get => _maxResults;
        set => _maxResults = Math.Clamp(value ?? 1, 1, 50);
    }
    
    /* Specifies the order in which search results should be returned */
    [QueryParameter("order")]
    public string? Order { get; private set; }

    /* Query string to be sent with the request */
    [QueryParameter("q")]
    [ValidatePropertyNotNull]
    public string Query { get; set; } = string.Empty;
    
    /* Specifies the type of resource we want to search for (videos, channels, playlists) */
    [QueryParameter("type")]
    public string? Type { get; private set; }
    
    internal YoutubeSearchServiceRequest(IClientService service) : base(service)
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