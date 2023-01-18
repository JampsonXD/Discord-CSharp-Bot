using Discord_CSharp_Bot.modules.youtube_api.endpoint.interfaces;
namespace Discord_CSharp_Bot.modules.youtube_api.request.interfaces;

public interface IYoutubeClient
{
    public IYoutubeChannelEndpoint YoutubeChannelEndpoint { get; }
    public IYoutubePlaylistItemsEndpoint YoutubePlaylistItemsEndpoint { get; }
    public string GetApiKey();
    public string GetAccessToken();
}