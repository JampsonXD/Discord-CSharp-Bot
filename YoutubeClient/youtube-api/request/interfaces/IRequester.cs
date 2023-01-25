namespace YoutubeClient.youtube_api.request.interfaces;

public interface IRequester
{
    Task<HttpResponseMessage> CreateRequestAsync(IRequest request);

}