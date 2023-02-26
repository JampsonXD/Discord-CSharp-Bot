using ClientService.ClientService;

namespace ClientService.ServiceRequests.Interfaces;

/// <summary>
/// Represents a http service request for a client service. Service requests take in some set of data, execute a request on that data, and return a response object.
/// </summary>

public interface IServiceRequest<TResponse>
{
    /// <summary>
    /// The type of method to be performed for the request.
    /// </summary>
    public HttpMethod HttpMethod { get; }
    
    /// <summary>
    /// A collection of query parameters for the request.
    /// </summary>
    /// 
    public IList<string> UriParameters { get; }
    
    /// <summary>
    /// The path of the request relative to the base path from the client service.
    /// </summary>
    public string RelativePath { get; }
    
    /// <summary>
    /// The client service this request is being performed for.
    /// </summary>
    public IClientService ClientService { get; }

    /// <summary>
    /// Executes the request.
    /// </summary>
    /// <returns>A response object.</returns>
    public TResponse ExecuteRequest();

    /// <summary>
    /// Executes the request asynchronously.
    /// </summary>
    /// <returns>A response object.</returns>
    public Task<TResponse> ExecuteRequestAsync();
}