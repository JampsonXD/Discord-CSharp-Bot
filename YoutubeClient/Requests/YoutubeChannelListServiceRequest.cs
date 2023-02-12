using ClientService.ServiceRequests;
using YoutubeClient.Models;

namespace YoutubeClient.Requests;

public class YoutubeChannelListServiceRequest: YoutubeServiceRequest<YoutubeChannel>
{
    public override string RelativePath => "channels";
    
    [RequestQueryParameter("part", true)]
    public List<string> Parts { get; set; }
    
    [RequestQueryParameter("id", true)]
    public string? Id { get; set; }
    
    [RequestQueryParameter("forUsername")]
    public string? ForUsername { get; set; }

    public YoutubeChannelListServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
    {
        Parts = new List<string>();
    }
}