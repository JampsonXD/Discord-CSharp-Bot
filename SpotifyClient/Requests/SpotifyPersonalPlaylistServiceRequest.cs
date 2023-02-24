using ClientService.ClientService;
using ClientService.ServiceRequests;
using SpotifyClient.DataModels;

namespace SpotifyClient.Requests;

public class SpotifyPersonalPlaylistServiceRequest: SpotifyServiceRequest<SpotifyPlaylists>
{
    public override HttpMethod HttpMethod => HttpMethod.Get;
    
    public SpotifyPersonalPlaylistServiceRequest(IClientService service) : base(service)
    {
        
    }
    
    public override string RelativePath => "me/playlists";
}