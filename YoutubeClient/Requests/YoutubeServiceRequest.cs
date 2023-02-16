using ClientService.ServiceRequests;
using YoutubeClient.Models;

namespace YoutubeClient.Requests;

public abstract class YoutubeServiceRequest<T>: BaseServiceRequest<YoutubeResponseDataWrapper<T>>
{
    protected YoutubeServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
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