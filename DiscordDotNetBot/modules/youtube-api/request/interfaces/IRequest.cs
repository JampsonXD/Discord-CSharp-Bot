namespace Discord_CSharp_Bot.modules.youtube_api.request.interfaces;

public interface IRequest
{
    public bool ShouldUseHttps();
    public HttpMethod GetHttpMethod();
    public List<string>? GetUrlParameters();
    public Dictionary<string, string>? GetHeaderParameters();
    public string GetHost();
    public string GetRelativeUrl();
    public string? ETag { get; }
    public bool HasETag { get; }
};