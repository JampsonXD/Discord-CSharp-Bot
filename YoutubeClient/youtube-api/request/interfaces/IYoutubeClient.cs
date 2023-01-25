using YoutubeClient.youtube_api.endpoint.interfaces;

namespace YoutubeClient.youtube_api.request.interfaces;

public interface IYoutubeClient
{
    public IYoutubeChannelEndpoint YoutubeChannelEndpoint { get; }
    public IYoutubePlaylistItemsEndpoint YoutubePlaylistItemsEndpoint { get; }
    public IYoutubeVideoEndpoint YoutubeVideoEndpoint { get; }
    public string GetApiKey();
    public string GetAccessToken();
}