using Microsoft.Extensions.Configuration;
using YoutubeClient.youtube_api.endpoint.interfaces;
using YoutubeClient.youtube_api.request.interfaces;

namespace YoutubeClient.youtube_api;

public class YoutubeRequestClient : IYoutubeClient
{
    private readonly string _apiToken;
    private readonly string _apiKey;

    public IYoutubeChannelEndpoint YoutubeChannelEndpoint { get; }
    public IYoutubePlaylistItemsEndpoint YoutubePlaylistItemsEndpoint { get; }
    public IYoutubeVideoEndpoint YoutubeVideoEndpoint { get; }

    public YoutubeRequestClient(IYoutubeChannelEndpoint youtubeChannelEndpoint, IYoutubePlaylistItemsEndpoint youtubePlaylistItemsEndpoint,IYoutubeVideoEndpoint youtubeVideoEndpoint, IConfiguration configuration)
    {
        _apiKey = configuration["YoutubeApiKey"] ?? throw new InvalidOperationException("Config does not have YoutubeApiKey assigned!");
        _apiToken = configuration["YoutubeApiToken"] ??
                    throw new InvalidOperationException("Config does not have YoutubeApiToken assigned!");

        YoutubeChannelEndpoint = youtubeChannelEndpoint;
        YoutubePlaylistItemsEndpoint = youtubePlaylistItemsEndpoint;
        YoutubeVideoEndpoint = youtubeVideoEndpoint;
    }

    public string GetApiKey()
    {
        return _apiToken;
    }

    public string GetAccessToken()
    {
        return _apiKey;
    }
}