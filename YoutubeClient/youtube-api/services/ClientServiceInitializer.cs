namespace YoutubeClient.youtube_api.services;

public class ClientServiceInitializer
{
    public string ApiKey { get; set; }
    public string? ServiceName { get; set; }
    public string? BaseUri { get; set; }
    public HttpClient HttpClient { get; set; }
}