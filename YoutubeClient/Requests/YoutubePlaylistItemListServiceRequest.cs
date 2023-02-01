using ClientService.ServiceRequests;
using YoutubeClient.Models;

namespace YoutubeClient.Requests;

public class YoutubePlaylistItemListServiceRequest: YoutubeServiceRequest<YoutubePlaylistItem>
{
    private int? _maxResults;
    public override string RelativePath => "playlistItems";

    [RequestQueryParameter("playlistId", true)]
    public string? PlaylistId { get; set; }
    
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

    public YoutubePlaylistItemListServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
    {
        Parts = new List<string>();
    }
}