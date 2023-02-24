using YoutubeClient.Requests;
using YoutubeClient.Services;

namespace YoutubeClient.RequestResources;

/// <summary>
/// Represents a collection of methods for Youtube Playlist Items.
/// </summary>

public sealed class PlaylistItemResource
{
    /// <summary>
    /// The Youtube client used to create request instances.
    /// </summary>
    private readonly YoutubeClientService _youtubeClientService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaylistItemResource"/> class.
    /// </summary>
    internal PlaylistItemResource(YoutubeClientService youtubeClientService)
    {
        _youtubeClientService = youtubeClientService;
    }

    /// <summary>
    /// Creates a <see cref="YoutubePlaylistItemListServiceRequest"/> instance.
    /// </summary>
    /// <returns>The youtube playlist item list service request.</returns>
    public YoutubePlaylistItemListServiceRequest List()
    {
        return new YoutubePlaylistItemListServiceRequest(_youtubeClientService);
    }
}