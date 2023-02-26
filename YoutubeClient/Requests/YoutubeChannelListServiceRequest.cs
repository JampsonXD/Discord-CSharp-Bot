using System.ComponentModel.DataAnnotations;
using ClientService.ClientService;
using ClientService.Core.Validation;
using ClientService.ServiceRequests;
using YoutubeClient.Models;

namespace YoutubeClient.Requests;

public sealed class YoutubeChannelListServiceRequest: YoutubeServiceRequest<YoutubeChannel>
{
    public override HttpMethod HttpMethod => HttpMethod.Get;
    public override string RelativePath => "channels";
    
    [RequestQueryParameter("part")]
    [ValidatePropertyNotNull]
    public List<string> Parts { get; set; }
    
    [RequestQueryParameter("id")]
    [ValidatePropertyNotNull]
    public string? Id { get; set; }
    
    [RequestQueryParameter("forUsername")]
    public string? ForUsername { get; set; }

    internal YoutubeChannelListServiceRequest(IClientService service) : base(service)
    {
        Parts = new List<string>();
    }
}