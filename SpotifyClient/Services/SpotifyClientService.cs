using System.Diagnostics;
using ClientService.ClientService;
using Newtonsoft.Json;
using SpotifyClient.RequestResources;

namespace SpotifyClient.Services;

public class SpotifyClientService: IClientService
{
    public string ApiKey => string.Empty;
    public string ServiceName => "Spotify Client";
    public string BaseUri => "https://api.spotify.com/v1/";
    public HttpClient HttpClient { get; }
    public AlbumResource Album { get; }
    public TrackResource Track { get; }

    public SpotifyClientService(ClientServiceInitializer initializer)
    {
        HttpClient = initializer.HttpClient ?? new HttpClient();
        Album = new AlbumResource(this);
        Track = new TrackResource(this);
    }
    
    public T HandleHttpResponseMessage<T>(HttpResponseMessage responseMessage)
    {
        var responseObject = JsonConvert.DeserializeObject<T>(responseMessage.Content.ReadAsStringAsync().Result);
        Debug.Assert(responseObject != null, "responseObject != null");
        return responseObject;
    }
}