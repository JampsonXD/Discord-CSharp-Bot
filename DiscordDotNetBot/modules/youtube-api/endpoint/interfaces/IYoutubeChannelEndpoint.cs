using Discord_CSharp_Bot.modules.youtube_api.models;
using Discord_CSharp_Bot.modules.youtube_api.request.request_parameters;

namespace Discord_CSharp_Bot.modules.youtube_api.endpoint.interfaces;



public interface IYoutubeChannelEndpoint
{
    Task<YoutubeChannel> GetYoutubeChannelByIdAsync(string channelId);

    Task<YoutubeChannel> GetYoutubeChannelByUsernameAsync(string username);

    Task<YoutubeChannel> GetYoutubeChannelWithParametersAsync(YoutubeRequestParameters parameters);
}