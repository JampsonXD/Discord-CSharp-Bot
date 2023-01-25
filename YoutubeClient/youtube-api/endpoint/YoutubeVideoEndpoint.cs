using System.Net;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using YoutubeClient.youtube_api.endpoint.interfaces;
using YoutubeClient.youtube_api.exceptions;
using YoutubeClient.youtube_api.models;
using YoutubeClient.youtube_api.request;
using YoutubeClient.youtube_api.request.interfaces;
using YoutubeClient.youtube_api.request.request_parameters;

namespace YoutubeClient.youtube_api.endpoint;

public class YoutubeVideoEndpoint: IYoutubeVideoEndpoint
{
    private readonly string _youtubeHost = "youtube.googleapis.com/youtube/v3/";
    private readonly string _relativeUrl = "videos";
    private readonly IRequester _requester;
    private readonly IMemoryCache _memoryCache;

    public YoutubeVideoEndpoint(IRequester requester, IMemoryCache memoryCache)
    {
        _requester = requester;
        _memoryCache = memoryCache;
    }
    
    public async Task<YoutubeResponseDataWrapper<YoutubeVideo>> GetYoutubeVideoItemsWithParametersAsync(YoutubeRequestParameters parameters, string? eTag = null)
    {
        GetRequestBuilder builder = new GetRequestBuilder().WithHttps().WithHost(_youtubeHost).WithRelativeUrl(_relativeUrl)
            .AddUrlParameters(parameters.ConstructParameters());

        YoutubeResponseDataWrapper<YoutubeVideo>? cachedValue = null;
        if (eTag != null && _memoryCache.TryGetValue(eTag, out cachedValue))
        {
            builder.WithETag(eTag);
        }
        
        var response = await _requester.CreateRequestAsync(builder.Build());

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            return cachedValue ?? throw new YoutubeInvalidRequestException("Cached Response Object is invalid and was requested!");
        }
        
        var responseObject = JsonConvert.DeserializeObject<YoutubeResponseDataWrapper<YoutubeVideo>>(await response.Content.ReadAsStringAsync());
        if (responseObject != null)
        {
            _memoryCache.Set(responseObject.ETag, responseObject,
                new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(15)));
            return responseObject;
        }

        throw new YoutubeInvalidRequestException("Response Object was unable to be parsed!");
    }
}