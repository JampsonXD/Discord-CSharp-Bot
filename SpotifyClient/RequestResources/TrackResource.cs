using ClientService.ClientService;
using ClientService.ServiceRequests;
using SpotifyClient.Requests;

namespace SpotifyClient.RequestResources;

public class TrackResource
{
    private readonly IClientService _clientService;

    public TrackResource(IClientService clientService)
    {
        _clientService = clientService;
    }

    public SpotifyTrackServiceRequest TrackRequest(string? trackId = null)
    {
        return new SpotifyTrackServiceRequest(new ServiceRequestInitializer()
        {
            ClientService = _clientService,
            HttpMethod = HttpMethod.Get
        })
        {
            TrackId = trackId
        };
    }
}