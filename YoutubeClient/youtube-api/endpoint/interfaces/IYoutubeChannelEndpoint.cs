using YoutubeClient.youtube_api.models;
using YoutubeClient.youtube_api.request.request_parameters;

namespace YoutubeClient.youtube_api.endpoint.interfaces;



public interface IYoutubeChannelEndpoint
{
    Task<YoutubeChannel> GetYoutubeChannelByIdAsync(string channelId);

    Task<YoutubeChannel> GetYoutubeChannelByUsernameAsync(string username);

    Task<YoutubeChannel> GetYoutubeChannelWithParametersAsync(YoutubeRequestParameters parameters);
}