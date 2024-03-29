﻿using System.Diagnostics;
using System.Net;
using ClientService.ClientService;
using Newtonsoft.Json;
using YoutubeClient.Exceptions;
using YoutubeClient.RequestResources;
using SearchResource = YoutubeClient.RequestResources.SearchResource;

namespace YoutubeClient.Services;

public class YoutubeClientService: IClientService
{
    public string ApiKey { get; }
    public string ServiceName { get; }
    public string BaseUri { get; }
    public HttpClient HttpClient { get; }
    public PlaylistItemResource PlaylistItemResource { get; }
    public ChannelResource ChannelResource { get; }
    
    public VideoResource VideoResource { get; }
    public SearchResource SearchResource { get; }

    public YoutubeClientService(ClientServiceInitializer serviceInitializer)
    {
        Debug.Assert(serviceInitializer.ApiKey != null, "serviceInitializer.ApiKey != null");
        ApiKey = serviceInitializer.ApiKey;
        HttpClient = serviceInitializer.HttpClient ?? new HttpClient();
        ServiceName = "Youtube Client";
        BaseUri = "https://youtube.googleapis.com/youtube/v3/";
        PlaylistItemResource = new PlaylistItemResource(this);
        ChannelResource = new ChannelResource(this);
        VideoResource = new VideoResource(this);
        SearchResource = new SearchResource(this);
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