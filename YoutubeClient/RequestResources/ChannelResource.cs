using YoutubeClient.Requests;
using YoutubeClient.Services;

namespace YoutubeClient.RequestResources;

/// <summary>
/// Represents a collection of methods for a Youtube Channel.
/// </summary>

public sealed class ChannelResource
{
    /// <summary>
    /// The Youtube client used to create request instances.
    /// </summary>
    private readonly YoutubeClientService _youtubeClientService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChannelResource"/> class.
    /// </summary>
    internal ChannelResource(YoutubeClientService youtubeClientService)
    {
        _youtubeClientService = youtubeClientService;
    }

    /// <summary>
    /// Creates a <see cref="YoutubeChannelListServiceRequest"/> instance.
    /// </summary>
    /// <returns>The youtube channel list service request.</returns>
    public YoutubeChannelListServiceRequest List()
    {
        return new YoutubeChannelListServiceRequest(_youtubeClientService);
    }
}