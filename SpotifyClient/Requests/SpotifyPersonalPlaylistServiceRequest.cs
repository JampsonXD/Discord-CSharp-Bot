using ClientService.ServiceRequests;
using SpotifyClient.DataModels;

namespace SpotifyClient.Requests;

public class SpotifyPersonalPlaylistServiceRequest: SpotifyServiceRequest<SpotifyPlaylists>
{
    public SpotifyPersonalPlaylistServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
    {
        
    }

    public override string RelativePath => "me/playlists";
}