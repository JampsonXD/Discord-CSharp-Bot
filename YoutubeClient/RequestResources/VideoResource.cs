using ClientService.ServiceRequests;
using YoutubeClient.Requests;
using YoutubeClient.Services;

namespace YoutubeClient.RequestResources;

public class VideoResource
{
    private readonly YoutubeClientService _youtubeClientService;

    public VideoResource(YoutubeClientService youtubeClientService)
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