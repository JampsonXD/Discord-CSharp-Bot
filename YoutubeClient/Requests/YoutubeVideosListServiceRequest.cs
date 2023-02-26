using ClientService.ClientService;
using ClientService.Core.Validation;
using ClientService.ServiceRequests;
using YoutubeClient.Models;

namespace YoutubeClient.Requests;

public sealed class YoutubeVideosListServiceRequest: YoutubeServiceRequest<YoutubeVideo>
{
    private int? _maxResults;
    public override HttpMethod HttpMethod => HttpMethod.Get;
    public override string RelativePath => "videos";

    [RequestQueryParameter("id")]
    [ValidatePropertyNotNull]
    public List<string> Ids { get; set; }
    
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
    
    internal YoutubeVideosListServiceRequest(IClientService service) : base(service)
    {
        Ids = new List<string>();
        Parts = new List<string>();
    }
}