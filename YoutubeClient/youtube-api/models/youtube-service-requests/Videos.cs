using YoutubeClient.youtube_api.request;
using YoutubeClient.youtube_api.request.youtube_requests;
using YoutubeClient.youtube_api.services;

namespace YoutubeClient.youtube_api.models.youtube_service_requests;

public class Videos
{
    private readonly YoutubeClientService _youtubeClientService;

    public Videos(YoutubeClientService youtubeClientService)
    {
        _youtubeClientService = youtubeClientService;
    }

    public YoutubeVideosListServiceRequest List()
    {
        return new YoutubeVideosListServiceRequest(new ServiceRequestInitializer()
        {
            HttpMethod = HttpMethod.Get,
            ClientService = _youtubeClientService
        });
    }
}