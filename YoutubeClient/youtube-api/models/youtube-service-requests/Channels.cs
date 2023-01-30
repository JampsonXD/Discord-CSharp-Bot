using YoutubeClient.youtube_api.request;
using YoutubeClient.youtube_api.request.youtube_requests;
using YoutubeClient.youtube_api.services;

namespace YoutubeClient.youtube_api.models.youtube_service_requests;

public class Channels
{
    private readonly YoutubeClientService _youtubeClientService;

    public Channels(YoutubeClientService youtubeClientService)
    {
        _youtubeClientService = youtubeClientService;
    }

    public YoutubeChannelListServiceRequest List()
    {
        return new YoutubeChannelListServiceRequest(new ServiceRequestInitializer()
        {
            HttpMethod = HttpMethod.Get,
            ClientService = _youtubeClientService
        });
    }
}