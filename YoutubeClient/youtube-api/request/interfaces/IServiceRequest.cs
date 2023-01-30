using YoutubeClient.youtube_api.services;

namespace YoutubeClient.youtube_api.request.interfaces;

public interface IServiceRequest<TResponse>
{
    public HttpMethod HttpMethod { get; }
    public IList<string> UriParameters { get; }
    public string RelativePath { get; }
    public IClientService ClientService { get; }

    public TResponse ExecuteRequest();

    public Task<TResponse> ExecuteRequestAsync();
}