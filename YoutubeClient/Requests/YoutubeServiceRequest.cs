using ClientService.ClientService;
using ClientService.ServiceRequests;
using YoutubeClient.Models;

namespace YoutubeClient.Requests;

public abstract class YoutubeServiceRequest<T>: BaseServiceRequest<YoutubeResponseDataWrapper<T>>
{
    protected YoutubeServiceRequest(IClientService service) : base(service)
    {
        
    }

    protected override void AddStaticUriParameters(IList<string> parametersList)
    {
        base.AddStaticUriParameters(parametersList);
        if (!string.IsNullOrWhiteSpace(ClientService.ApiKey))
        {
            parametersList.Add($"key={ClientService.ApiKey}");
        }
    }
}