using Microsoft.Extensions.Caching.Memory;
using YoutubeClient.youtube_api.endpoint.interfaces;
using YoutubeClient.youtube_api.models;
using YoutubeClient.youtube_api.request.interfaces;
using YoutubeClient.youtube_api.request.request_parameters;

namespace YoutubeClient.youtube_api.endpoint;

public class YoutubeVideoEndpoint: BaseYoutubeEndpoint<YoutubeVideo>, IYoutubeVideoEndpoint
{
    public YoutubeVideoEndpoint(IRequester requester, IMemoryCache memoryCache): base(requester, memoryCache)
    {
        RelativeUrl = "videos";
    }
    
    public async Task<YoutubeResponseDataWrapper<YoutubeVideo>> GetYoutubeVideoItemsWithParametersAsync(YoutubeRequestParameters parameters, string? eTag = null)
    {
        return await GetYoutubeResponseData(parameters, eTag);
    }
}