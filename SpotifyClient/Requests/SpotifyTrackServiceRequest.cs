using ClientService.ClientService;
using ClientService.ServiceRequests;
using SpotifyClient.DataModels;

namespace SpotifyClient.Requests;

public class SpotifyTrackServiceRequest: SpotifyServiceRequest<SpotifyTrack>
{
    public override HttpMethod HttpMethod => HttpMethod.Get;
    public string? TrackId { get; set; }
    public override string RelativePath => $"tracks/{TrackId}";
    
    public SpotifyTrackServiceRequest(IClientService service) : base(service)
    {
        
    }
}