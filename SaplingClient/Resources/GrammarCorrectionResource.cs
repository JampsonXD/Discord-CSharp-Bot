using ClientService.ServiceRequests;
using SaplingClient.Requests;
using SaplingClient.Services;

namespace SaplingClient.Resources;

public class GrammarCorrectionResource
{
    private readonly SaplingClientService _service;

    public GrammarCorrectionResource(SaplingClientService service)
    {
        _service = service;
    }

    public GrammarCheckingServiceRequest GrammarCheckingService()
    {
        return new GrammarCheckingServiceRequest(new ServiceRequestInitializer()
        {
            HttpMethod = HttpMethod.Post,
            ClientService = _service
        });
    }
}