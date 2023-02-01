using ClientService.ServiceRequests;
using YoutubeClient.Models;

namespace YoutubeClient.Requests;

public class YoutubeVideosListServiceRequest: YoutubeServiceRequest<YoutubeVideo>
{
    private int? _maxResults;

    public override string RelativePath => "videos";

    [RequestQueryParameter("id", true)]
    public List<string> Ids { get; set; }
    
    [RequestQueryParameter("part", true)]
    public List<string> Parts { get; set; }

    [RequestQueryParameter("maxResults")]
    public int? MaxResults
    {
        get => _maxResults;
        set => _maxResults = Math.Clamp(value ?? 1, 1, 50);
    }
    
    [RequestQueryParameter("pageToken")]
    public string? PageToken { get; set; }
    
    public YoutubeVideosListServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
    {
        Ids = new List<string>();
        Parts = new List<string>();
    }
}