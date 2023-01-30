using YoutubeClient.youtube_api.request;
using YoutubeClient.youtube_api.request.youtube_requests;
using YoutubeClient.youtube_api.services;

namespace YoutubeClient.youtube_api.models.youtube_service_requests;

public class PlaylistItems
{
    private readonly YoutubeClientService _youtubeClientService;

    public PlaylistItems(YoutubeClientService youtubeClientService)
    {
        _youtubeClientService = youtubeClientService;
    }

    public YoutubePlaylistItemListServiceRequest List()
    {
        return new YoutubePlaylistItemListServiceRequest(new ServiceRequestInitializer()
        {
            HttpMethod = HttpMethod.Get,
            ClientService = _youtubeClientService
        });
    }
}