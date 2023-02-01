using ClientService.ClientService;
using ClientService.ServiceRequests;
using SpotifyClient.Requests;

namespace SpotifyClient.RequestResources;

public class AlbumResource
{
    private readonly IClientService _clientService;

    public AlbumResource(IClientService clientService)
    {
        _clientService = clientService;
    }

    public SpotifyAlbumServiceRequest AlbumRequest()
    {
        return new SpotifyAlbumServiceRequest(new ServiceRequestInitializer()
        {
            ClientService = _clientService,
            HttpMethod = HttpMethod.Get
        });
    }
}