using YoutubeClient.youtube_api.models.youtube_data_models;

namespace YoutubeClient.youtube_api.request.youtube_requests;

public class YoutubeChannelListServiceRequest: YoutubeServiceRequest<YoutubeChannel>
{
    [RequestParameter("part", true)]
    public List<string> Parts { get; set; }
    
    [RequestParameter("id", true)]
    public string Id { get; set; }
    
    [RequestParameter("forUsername")]
    public string? ForUsername { get; set; }

    public YoutubeChannelListServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
    {
        Parts = new List<string>();
        RelativePath = "channels";
    }
}