using ClientService.ServiceRequests;
using YoutubeClient.Requests;
using YoutubeClient.Services;

namespace YoutubeClient.RequestResources;

public class PlaylistItemResource
{
    private readonly YoutubeClientService _youtubeClientService;

    public PlaylistItemResource(YoutubeClientService youtubeClientService)
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