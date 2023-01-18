﻿using Discord_CSharp_Bot.modules.youtube_api.endpoint;
using Discord_CSharp_Bot.modules.youtube_api.endpoint.interfaces;
using Discord_CSharp_Bot.modules.youtube_api.models;
using Discord_CSharp_Bot.modules.youtube_api.request;
using Discord_CSharp_Bot.modules.youtube_api.request.interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Discord_CSharp_Bot.modules.youtube_api;

public class YoutubeRequestClient : IYoutubeClient
{
    private readonly string _apiToken;
    private readonly string _apiKey;

    public IYoutubeChannelEndpoint YoutubeChannelEndpoint { get; }
    public IYoutubePlaylistItemsEndpoint YoutubePlaylistItemsEndpoint { get; }

    public YoutubeRequestClient(IYoutubeChannelEndpoint youtubeChannelEndpoint, IYoutubePlaylistItemsEndpoint youtubePlaylistItemsEndpoint, IConfiguration configuration)
    {
        _apiKey = configuration["YoutubeApiKey"] ?? throw new InvalidOperationException("Config does not have YoutubeApiKey assigned!");
        _apiToken = configuration["YoutubeApiToken"] ??
                    throw new InvalidOperationException("Config does not have YoutubeApiToken assigned!");

        YoutubeChannelEndpoint = youtubeChannelEndpoint;
        YoutubePlaylistItemsEndpoint = youtubePlaylistItemsEndpoint;
    }

    public string GetApiKey()
    {
        return _apiToken;
    }

    public string GetAccessToken()
    {
        return _apiKey;
    }
}