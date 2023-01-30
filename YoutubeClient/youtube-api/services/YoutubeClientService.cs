using System.Net;
using Newtonsoft.Json;
using YoutubeClient.youtube_api.exceptions;
using YoutubeClient.youtube_api.models.youtube_service_requests;

namespace YoutubeClient.youtube_api.services;

public class YoutubeClientService: IClientService
{
    public string ApiKey { get; }
    public string ServiceName { get; }
    public string BaseUri { get; }
    public HttpClient HttpClient { get; }
    public PlaylistItems PlaylistItems { get; }
    public Channels Channels { get; }
    
    public Videos Videos { get; }

    public YoutubeClientService(ClientServiceInitializer serviceInitializer)
    {
        ApiKey = serviceInitializer.ApiKey;
        HttpClient = serviceInitializer.HttpClient;
        ServiceName = "Youtube Client";
        BaseUri = "https://youtube.googleapis.com/youtube/v3/";
        PlaylistItems = new PlaylistItems(this);
        Channels = new Channels(this);
        Videos = new Videos(this);
    }
    
    public T HandleHttpResponseMessage<T>(HttpResponseMessage response)
    {
        /* Make sure our response code is either successful or that the content was not modified,
           Status code 304 Not Modified will still return a failure status code */
        if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotModified)
        {
            throw new YoutubeInvalidRequestException($"Youtube Request failed.\n" +
                                                     $"Status Code: {response.StatusCode}\n" +
                                                     $"Error Reason: {response.ReasonPhrase}");
        }

        var value = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
        if (value == null)
        {
            throw new YoutubeInvalidRequestException($"Youtube Request failed.\n" +
                                                     $"Unable to deserialize response into a valid object.");
        }

        return value;
    }
}