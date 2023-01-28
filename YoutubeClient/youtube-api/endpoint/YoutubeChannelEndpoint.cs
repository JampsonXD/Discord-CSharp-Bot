using Microsoft.Extensions.Caching.Memory;
using YoutubeClient.youtube_api.endpoint.interfaces;
using YoutubeClient.youtube_api.models;
using YoutubeClient.youtube_api.request.interfaces;
using YoutubeClient.youtube_api.request.request_parameters;

namespace YoutubeClient.youtube_api.endpoint;

public class YoutubeChannelEndpoint : BaseYoutubeEndpoint<YoutubeChannel>, IYoutubeChannelEndpoint
{
    public YoutubeChannelEndpoint(IRequester requester, IMemoryCache memoryCache): base(requester, memoryCache)
    {
        RelativeUrl = "channels";
    }
    
    public async Task<YoutubeChannel> GetYoutubeChannelByIdAsync(string channelId)
    {
        YoutubeChannelParameterBuilder builder = new YoutubeChannelParameterBuilder();
        builder.WithContentDetails().WithId(channelId);

        return await GetYoutubeChannelWithParametersAsync(builder.Build());
    }

    public async Task<YoutubeChannel> GetYoutubeChannelByUsernameAsync(string username)
    {
        YoutubeChannelParameterBuilder builder = new YoutubeChannelParameterBuilder();
        builder.WithContentDetails().WithUsername(username);

        return await GetYoutubeChannelWithParametersAsync(builder.Build());
    }

    public async Task<YoutubeChannel> GetYoutubeChannelWithParametersAsync(YoutubeRequestParameters parameters)
    {
        var response = await GetYoutubeResponseData(parameters, null);
        return response.Items.First();
    }
}