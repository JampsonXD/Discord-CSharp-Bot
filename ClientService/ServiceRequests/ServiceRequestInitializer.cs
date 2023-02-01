using ClientService.ClientService;

namespace ClientService.ServiceRequests;

public class ServiceRequestInitializer
{
    public HttpMethod HttpMethod { get; set; }
    public IClientService ClientService { get; set; }
}