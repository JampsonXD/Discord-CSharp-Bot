using ClientService.ClientService;

namespace ClientService.ServiceRequests;

public class ServiceRequestInitializer
{
    public required HttpMethod HttpMethod { get; set; }
    public required IClientService ClientService { get; set; }
}