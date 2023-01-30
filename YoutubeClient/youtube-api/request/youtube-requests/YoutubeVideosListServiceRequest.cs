using YoutubeClient.youtube_api.models.youtube_data_models;

namespace YoutubeClient.youtube_api.request.youtube_requests;

public class YoutubeVideosListServiceRequest: YoutubeServiceRequest<YoutubeVideo>
{
    private int? _maxResults;

    [RequestParameter("id", true)]
    public List<string> Ids { get; set; }
    
    [RequestParameter("part", true)]
    public List<string> Parts { get; set; }

    [RequestParameter("maxResults")]
    public int? MaxResults
    {
        get => _maxResults;
        set => _maxResults = Math.Clamp(value ?? 1, 1, 50);
    }
    
    [RequestParameter("pageToken")]
    public string? PageToken { get; set; }
    
    public YoutubeVideosListServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
    {
        RelativePath = "videos";
        Ids = new List<string>();
        Parts = new List<string>();
    }
}