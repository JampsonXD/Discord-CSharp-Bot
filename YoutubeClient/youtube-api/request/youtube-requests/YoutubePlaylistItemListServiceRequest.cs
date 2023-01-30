﻿using YoutubeClient.youtube_api.models.youtube_data_models;

namespace YoutubeClient.youtube_api.request.youtube_requests;

public class YoutubePlaylistItemListServiceRequest: YoutubeServiceRequest<YoutubePlaylistItem>
{
    private int? _maxResults;

    [RequestParameter("playlistId", true)]
    public string? PlaylistId { get; set; }
    
    [RequestParameter("part", true)]
    public List<string> Parts { get; set; }

    [RequestParameter("maxResults")]
    public int? MaxResults
    {
        get => _maxResults;
        set => _maxResults = Math.Clamp(value ?? 1, 1, 50);
    }
    
    [RequestParameter("pageToken")]
    public string? PageToken { get; set; }

    public YoutubePlaylistItemListServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
    {
        Parts = new List<string>();
        RelativePath = "playlistItems";
    }
}