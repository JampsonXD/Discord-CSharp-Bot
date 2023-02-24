using ClientService.ClientService;
using ClientService.ServiceRequests;
using SpotifyClient.DataModels;

namespace SpotifyClient.Requests;

public class SpotifyAlbumServiceRequest: SpotifyServiceRequest<SpotifyAlbum>
{
    public override HttpMethod HttpMethod => HttpMethod.Get;
    public override string RelativePath => $"albums/{AlbumId}";
    
    public string? AlbumId { get; set; }
    
    public SpotifyAlbumServiceRequest(IClientService service) : base(service)
    {
        
    }
}