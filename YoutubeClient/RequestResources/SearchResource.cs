using YoutubeClient.Requests;
using YoutubeClient.Services;

namespace YoutubeClient.RequestResources;

/// <summary>
/// Represents a collection of methods for creating Youtube search requests.
/// </summary>

public sealed class SearchResource
{
    /// <summary>
    /// The Youtube client used to create request instances.
    /// </summary>
    private readonly YoutubeClientService _youtubeClientService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchResource"/> class.
    /// </summary>
    internal SearchResource(YoutubeClientService youtubeClientService)
    {
        _youtubeClientService = youtubeClientService;
    }

    /// <summary>
    /// Creates a <see cref="YoutubeSearchServiceRequest"/> instance.
    /// </summary>
    /// <returns>The youtube search service request</returns>
    public YoutubeSearchServiceRequest List()
    {
        return new YoutubeSearchServiceRequest(_youtubeClientService);
    }
}