using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using YoutubeClient.youtube_api.endpoint.interfaces;
using YoutubeClient.youtube_api.models;
using YoutubeClient.youtube_api.request;
using YoutubeClient.youtube_api.request.interfaces;
using YoutubeClient.youtube_api.request.request_parameters;

namespace YoutubeClient.youtube_api.endpoint;

public class YoutubeChannelEndpoint : IYoutubeChannelEndpoint
{
    private readonly string _youtubeHost = "youtube.googleapis.com/youtube/v3/";
    private readonly string _relativeUrl = "channels";
    private readonly IRequester _requester;

    public YoutubeChannelEndpoint(IRequester requester, IConfiguration configuration)
    {
        _requester = requester;
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
        // Construct parameters
        var paramList = parameters.ConstructParameters();

        var builder = new GetRequestBuilder().WithHost(_youtubeHost).WithRelativeUrl(_relativeUrl)
            .WithHttps().AddUrlParameters(paramList);

        var response =
            await _requester.CreateRequestAsync(
                builder.Build());

        var channel = JsonConvert.DeserializeObject<YoutubeResponseDataWrapper<YoutubeChannel>>(await response.Content.ReadAsStringAsync()).Items.First();
        return channel;
    }
}