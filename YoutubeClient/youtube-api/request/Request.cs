using YoutubeClient.youtube_api.request.interfaces;

namespace YoutubeClient.youtube_api.request;

/* Represents a Get Request that would be sent across a network */
public class GetRequest : IRequest
{
    private bool UseHttps { get; }
    private List<string>? UrlParameters { get; }
    private Dictionary<string, string>? HeaderParameters { get; }
    private string Host { get; }
    private string RelativeUrl { get; }
    public string? ETag { get; }
    public bool HasETag => !string.IsNullOrEmpty(ETag);

    public GetRequest(bool useHttps, List<string>? urlParameters, Dictionary<string, string>? headerParameters, string host, string relativeUrl, string? eTag = null)
    {
        UseHttps = useHttps;
        UrlParameters = urlParameters;
        HeaderParameters = headerParameters;
        Host = host;
        RelativeUrl = relativeUrl;
        ETag = eTag;
    }
    
    public bool ShouldUseHttps()
    {
        return UseHttps;
    }

    public HttpMethod GetHttpMethod()
    {
        return HttpMethod.Get;
    }

    public List<string>? GetUrlParameters()
    {
        return UrlParameters;
    }

    public Dictionary<string, string>? GetHeaderParameters()
    {
        return HeaderParameters;
    }

    public string GetHost()
    {
        return Host;
    }

    public string GetRelativeUrl()
    {
        return RelativeUrl;
    }
}

/* Generic Builder Class for Requests */
public interface IRequestBuilder<T> where T : IRequest
{
    // Build/Validate/Return an item of type T
    T Build();
}


/* Request Builder for creating a GetRequest */
public class GetRequestBuilder : IRequestBuilder<GetRequest>
{
    private bool _useHttps;
    private List<string>? _urlParameters;
    private Dictionary<string, string>? _headerParameters;
    private string _host;
    private string _relativeUrl;
    private string? _eTag;
    
    public GetRequestBuilder()
    {
        _useHttps = true;
        _urlParameters = null;
        _headerParameters = null;
        _host = "";
        _relativeUrl = "";
        _eTag = null;
    }

    public GetRequestBuilder WithHttp()
    {
        _useHttps = false;
        return this;
    }

    public GetRequestBuilder WithHttps()
    {
        _useHttps = true;
        return this;
    }

    public GetRequestBuilder AddUrlParameter(string parameter)
    {
        GetUrlParameterList().Add(parameter);
        return this;
    }

    public GetRequestBuilder AddUrlParameters(IList<string> parameters)
    {
        GetUrlParameterList().AddRange(parameters);
        return this;
    }

    public GetRequestBuilder AddHeaderParameter(string key, string value)
    {
        GetHeaderDictionary().Add(key, value);
        return this;
    }

    public GetRequestBuilder AddHeaderParameters(Dictionary<string, string> parameters)
    {
        GetHeaderDictionary().ToList().ForEach(pair => GetHeaderDictionary()[pair.Key] = pair.Value);
        return this;
    }

    public GetRequestBuilder WithHost(string host)
    {
        _host = host;
        return this;
    }

    public GetRequestBuilder WithRelativeUrl(string relativeUrl)
    {
        _relativeUrl = relativeUrl;
        return this;
    }

    public GetRequestBuilder WithETag(string? eTag)
    {
        _eTag = eTag;
        return this;
    }
    
    private List<string> GetUrlParameterList()
    {
        if (_urlParameters == null)
        {
            _urlParameters = new List<string>();
        }

        return _urlParameters;
    }

    private Dictionary<string, string> GetHeaderDictionary()
    {
        if (_headerParameters == null)
        {
            _headerParameters = new Dictionary<string, string>();
        }

        return _headerParameters;
    }

    public GetRequest Build()
    {
        var request = new GetRequest(_useHttps, _urlParameters, _headerParameters, _host, _relativeUrl, _eTag);
        Validate(request);
        return request;
    }

    // Validate that the item was built properly and is valid
    private void Validate(GetRequest request)
    {
        // Host shouldn't be null and should have an address to send
        if (string.IsNullOrEmpty(request.GetHost()))
        {
            throw new Exception($"Request {request} contains an invalid host string!");
        }
    }
}