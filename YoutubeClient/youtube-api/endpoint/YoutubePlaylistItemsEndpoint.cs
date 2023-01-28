using Microsoft.Extensions.Caching.Memory;
using YoutubeClient.youtube_api.endpoint.interfaces;
using YoutubeClient.youtube_api.models;
using YoutubeClient.youtube_api.request.interfaces;
using YoutubeClient.youtube_api.request.request_parameters;

namespace YoutubeClient.youtube_api.endpoint;

public class YoutubePlaylistItemsEndpoint: BaseYoutubeEndpoint<YoutubePlaylistItem>, IYoutubePlaylistItemsEndpoint
{
    public YoutubePlaylistItemsEndpoint(IRequester requester, IMemoryCache memoryCache): base(requester, memoryCache)
    {
        RelativeUrl = "playlistItems";
    }
    
    public async Task<YoutubeResponseDataWrapper<YoutubePlaylistItem>> GetYoutubePlaylistItemsByPlaylistIdAsync(string playlistId, int maxResults, string? eTag = null)
    {
        var parameterBuilder = new YoutubePlaylistItemParameterBuilder().WithSnippet().WithId(playlistId)
            .WithMaxResults(maxResults);

        return await GetYoutubePlaylistItemsWithParametersAsync(parameterBuilder.Build(), eTag);
    }

    public async Task<YoutubeResponseDataWrapper<YoutubePlaylistItem>> GetYoutubePlaylistItemsWithParametersAsync(YoutubeRequestParameters parameters, string? eTag = null)
    {
        return await GetYoutubeResponseData(parameters, eTag);
    }
}