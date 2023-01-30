using YoutubeClient.youtube_api.models.youtube_data_models;

namespace YoutubeClient.youtube_api.request;

public class YoutubeServiceRequest<T>: BaseServiceRequest<YoutubeResponseDataWrapper<T>>
{
    public string? ETag { get; set; }
    
    public YoutubeServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
    {
        
    }
}