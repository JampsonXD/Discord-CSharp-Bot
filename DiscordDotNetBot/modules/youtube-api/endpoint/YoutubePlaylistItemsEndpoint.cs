using System.Net;
using Discord_CSharp_Bot.modules.youtube_api.endpoint.interfaces;
using Discord_CSharp_Bot.modules.youtube_api.exceptions;
using Discord_CSharp_Bot.modules.youtube_api.models;
using Discord_CSharp_Bot.modules.youtube_api.request;
using Discord_CSharp_Bot.modules.youtube_api.request.interfaces;
using Discord_CSharp_Bot.modules.youtube_api.request.request_parameters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Discord_CSharp_Bot.modules.youtube_api.endpoint;

public class YoutubePlaylistItemsEndpoint: IYoutubePlaylistItemsEndpoint
{
    private readonly string _youtubeHost = "youtube.googleapis.com/youtube/v3/";
    private readonly string _relativeUrl = "playlistItems";
    private readonly IRequester _requester;
    private readonly IMemoryCache _memoryCache;

    public YoutubePlaylistItemsEndpoint(IRequester requester, IConfiguration configuration, IMemoryCache memoryCache)
    {
        _requester = requester;
        _memoryCache = memoryCache;
    }
    
    public async Task<YoutubeResponseDataWrapper<YoutubePlaylistItem>> GetYoutubePlaylistItemsByPlaylistIdAsync(string playlistId, int maxResults, string? eTag = null)
    {
        var parameterBuilder = new YoutubePlaylistItemParameterBuilder().WithSnippet().WithId(playlistId)
            .WithMaxResults(maxResults);

        return await GetYoutubePlaylistItemsWithParametersAsync(parameterBuilder.Build(), eTag);
    }

    public async Task<YoutubeResponseDataWrapper<YoutubePlaylistItem>> GetYoutubePlaylistItemsWithParametersAsync(YoutubeRequestParameters parameters, string? eTag = null)
    {
        GetRequestBuilder builder = new GetRequestBuilder().WithHttps().WithHost(_youtubeHost).WithRelativeUrl(_relativeUrl)
            .AddUrlParameters(parameters.ConstructParameters());

        YoutubeResponseDataWrapper<YoutubePlaylistItem>? cachedValue = null;
        if (eTag != null && _memoryCache.TryGetValue(eTag, out cachedValue))
        {
            builder.WithETag(eTag);
        }
        
        var response = await _requester.CreateRequestAsync(builder.Build());

        if (response.StatusCode == HttpStatusCode.NotModified)
        {
            return cachedValue ?? throw new YoutubeInvalidRequestException("Cached Response Object is invalid and was requested!");
        }
        
        var responseObject = JsonConvert.DeserializeObject<YoutubeResponseDataWrapper<YoutubePlaylistItem>>(await response.Content.ReadAsStringAsync());
        if (responseObject != null)
        {
            _memoryCache.Set(responseObject.ETag, responseObject,
                new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(15)));
            return responseObject;
        }

        throw new YoutubeInvalidRequestException("Response Object was unable to be parsed!");
    }
}