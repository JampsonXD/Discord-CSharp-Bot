using ClientService.ServiceRequests;
using YoutubeClient.Requests;
using YoutubeClient.Services;

namespace YoutubeClient.RequestResources;

public class SearchResource
{
    private readonly YoutubeClientService _youtubeClientService;

    public SearchResource(YoutubeClientService youtubeClientService)
    {
        _youtubeClientService = youtubeClientService;
    }

    public YoutubeSearchServiceRequest List()
    {
        return new YoutubeSearchServiceRequest(new ServiceRequestInitializer()
        {
            HttpMethod = HttpMethod.Get,
            ClientService = _youtubeClientService
        });
    }
}