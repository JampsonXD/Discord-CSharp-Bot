using YoutubeClient.youtube_api.models;
using YoutubeClient.youtube_api.request.request_parameters;

namespace YoutubeClient.youtube_api.endpoint.interfaces;

public interface IYoutubeVideoEndpoint
{
    public Task<YoutubeResponseDataWrapper<YoutubeVideo>> GetYoutubeVideoItemsWithParametersAsync(
        YoutubeRequestParameters parameters, string? eTag = null);
}