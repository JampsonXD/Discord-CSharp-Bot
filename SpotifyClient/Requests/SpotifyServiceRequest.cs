using ClientService.ClientService;
using ClientService.ServiceRequests;

namespace SpotifyClient.Requests;

public abstract class SpotifyServiceRequest<T>: BaseServiceRequest<T>
{
    protected SpotifyServiceRequest(IClientService service) : base(service)
    {
        
    }
}