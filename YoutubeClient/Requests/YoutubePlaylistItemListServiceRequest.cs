using ClientService.ClientService;
using ClientService.Core.Validation;
using ClientService.ServiceRequests;
using YoutubeClient.Models;

namespace YoutubeClient.Requests;

public sealed class YoutubePlaylistItemListServiceRequest: YoutubeServiceRequest<YoutubePlaylistItem>
{
    private int? _maxResults;
    public override HttpMethod HttpMethod => HttpMethod.Get;
    public override string RelativePath => "playlistItems";

    [RequestQueryParameter("playlistId")]
    [ValidatePropertyNotNull]
    public string? PlaylistId { get; set; }
    
    [RequestQueryParameter("part")]
    [ValidatePropertyNotNull]
    public List<string> Parts { get; set; }

    [RequestQueryParameter("maxResults")]
    public int? MaxResults
    {
        get => _maxResults;
        set => _maxResults = Math.Clamp(value ?? 1, 1, 50);
    }
    
    [RequestQueryParameter("pageToken")]
    public string? PageToken { get; set; }

    internal YoutubePlaylistItemListServiceRequest(IClientService service) : base(service)
    {
        Parts = new List<string>();
    }
}