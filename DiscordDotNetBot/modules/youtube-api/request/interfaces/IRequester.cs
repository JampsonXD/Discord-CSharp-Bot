namespace Discord_CSharp_Bot.modules.youtube_api.request.interfaces;

public interface IRequester
{
    Task<HttpResponseMessage> CreateRequestAsync(IRequest request);

}