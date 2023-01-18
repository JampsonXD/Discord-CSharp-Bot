using Discord_CSharp_Bot.modules.youtube_api.models;
using Discord_CSharp_Bot.modules.youtube_api.request.request_parameters;

namespace Discord_CSharp_Bot.modules.youtube_api.endpoint.interfaces;

public interface IYoutubePlaylistItemsEndpoint
{
    public Task<YoutubeResponseDataWrapper<YoutubePlaylistItem>> GetYoutubePlaylistItemsByPlaylistIdAsync(string playlistId, int maxResults, string? eTag = null);

    public Task<YoutubeResponseDataWrapper<YoutubePlaylistItem>> GetYoutubePlaylistItemsWithParametersAsync(
        YoutubeRequestParameters parameters, string? eTag = null);
}