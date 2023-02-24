using ClientService.ClientService;
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
        return new SpotifyTrackServiceRequest(_clientService)
        {
            TrackId = trackId
        };
    }
}