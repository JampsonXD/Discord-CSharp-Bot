﻿namespace ClientService.ClientService;

public interface IClientService
{
    public string ApiKey { get; }
    public string ServiceName { get; }
    public string BaseUri { get; }
    public HttpClient HttpClient { get; }

    public T HandleHttpResponseMessage<T>(HttpResponseMessage responseMessage);
}