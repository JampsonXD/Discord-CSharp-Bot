using YoutubeClient.youtube_api.services;

namespace YoutubeClient.youtube_api.request;

public class ServiceRequestInitializer
{
    public HttpMethod HttpMethod { get; set; }
    public IList<string>? UriParameters { get; set; }
    public string? RelativePath { get; set; }
    public IClientService ClientService { get; set; }
}