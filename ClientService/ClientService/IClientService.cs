using ClientService.ServiceRequests.Interfaces;

namespace ClientService.ClientService;

/// <summary>
/// Represents the necessary information for a client service.
/// </summary>
/// <remarks>
/// Client services contain all the necessary information needed to interact with a service, such as an ApiKey and a BaseUri for resources.
/// </remarks>

public interface IClientService
{
    /// <summary>
    /// Gets the ApiKey string.
    /// </summary>
    public string ApiKey { get; }
    
    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    public string ServiceName { get; }
    
    /// <summary>
    /// Gets the BaseUri for making requests.
    /// </summary>
    public string BaseUri { get; }
    
    /// <summary>
    /// Gets an HttpClient for making requests for the service.
    /// </summary>
    public HttpClient HttpClient { get; }

    /// <summary>
    /// Handles <see cref="HttpResponseMessage"/> messages.
    /// </summary>
    /// <typeparam name="T">The </typeparam>
    /// <param name="responseMessage">The response message</param>
    /// <returns>The deserialized response object.</returns>
    public T HandleHttpResponseMessage<T>(HttpResponseMessage responseMessage);
}