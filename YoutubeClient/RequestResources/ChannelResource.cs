using ClientService.ServiceRequests;
using YoutubeClient.Requests;
using YoutubeClient.Services;

namespace YoutubeClient.RequestResources;

public class ChannelResource
{
    private readonly YoutubeClientService _youtubeClientService;

    public ChannelResource(YoutubeClientService youtubeClientService)
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