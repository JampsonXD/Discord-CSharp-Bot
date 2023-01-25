using YoutubeClient.youtube_api.models;
using YoutubeClient.youtube_api.request.request_parameters;

namespace YoutubeClient.youtube_api.endpoint.interfaces;

public interface IYoutubePlaylistItemsEndpoint
{
    public Task<YoutubeResponseDataWrapper<YoutubePlaylistItem>> GetYoutubePlaylistItemsByPlaylistIdAsync(string playlistId, int maxResults, string? eTag = null);

    public Task<YoutubeResponseDataWrapper<YoutubePlaylistItem>> GetYoutubePlaylistItemsWithParametersAsync(
        YoutubeRequestParameters parameters, string? eTag = null);
}