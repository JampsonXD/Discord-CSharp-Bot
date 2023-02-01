using ClientService.ServiceRequests;
using YoutubeClient.Models;

namespace YoutubeClient.Requests;

public abstract class YoutubeServiceRequest<T>: BaseServiceRequest<YoutubeResponseDataWrapper<T>>
{
    protected YoutubeServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
    {
        
    }
}