using YoutubeClient.Requests;
using YoutubeClient.Services;

namespace YoutubeClient.RequestResources;

/// <summary>
/// Represents a collection of methods for Youtube videos.
/// </summary>

public sealed class VideoResource
{
    /// <summary>
    /// The Youtube client used to create request instances.
    /// </summary>
    private readonly YoutubeClientService _youtubeClientService;

    /// <summary>
    /// Initializes a new instance of the <see cref="VideoResource"/> class.
    /// </summary>
    internal VideoResource(YoutubeClientService youtubeClientService)
    {
        _youtubeClientService = youtubeClientService;
    }

    /// <summary>
    /// Creates a <see cref="YoutubeVideosListServiceRequest"/> instance.
    /// </summary>
    /// <returns>The youtube videos list service request</returns>
    public YoutubeVideosListServiceRequest List()
    {
        return new YoutubeVideosListServiceRequest(_youtubeClientService);
    }
}