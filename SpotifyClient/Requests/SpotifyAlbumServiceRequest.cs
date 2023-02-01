using ClientService.ServiceRequests;
using SpotifyClient.DataModels;

namespace SpotifyClient.Requests;

public class SpotifyAlbumServiceRequest: SpotifyServiceRequest<SpotifyAlbum>
{
    public override string RelativePath => $"albums/{AlbumId}";
    
    public string? AlbumId { get; set; }
    
    public SpotifyAlbumServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
    {
        
    }
}