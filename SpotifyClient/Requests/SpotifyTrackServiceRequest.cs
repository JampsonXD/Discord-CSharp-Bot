using ClientService.ServiceRequests;
using SpotifyClient.DataModels;

namespace SpotifyClient.Requests;

public class SpotifyTrackServiceRequest: SpotifyServiceRequest<SpotifyTrack>
{
    public string? TrackId { get; set; }
    public override string RelativePath => $"tracks/{TrackId}";
    
    public SpotifyTrackServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
    {
        
    }
}