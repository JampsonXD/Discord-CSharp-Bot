using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using YoutubeClient.youtube_api.exceptions;
using YoutubeClient.youtube_api.request.interfaces;

namespace YoutubeClient.youtube_api.request;

public class YoutubeRequester : IRequester
{
    private readonly HttpClient _client;
    private readonly string _apiKey;

    public YoutubeRequester(HttpClient client, IConfiguration configuration)
    {
        _client = client;
        _apiKey = configuration["YoutubeApiKey"] ?? throw new InvalidOperationException($"{this.ToString()} failed to find YoutubeApiKey in configuration service!");
    }
    public async Task<HttpResponseMessage> CreateRequestAsync(IRequest request)
    {
        var httpRequest = await PrepareRequestAsync(request);
        return await SendRequestAsync(httpRequest);
    }

    private async Task<HttpRequestMessage> PrepareRequestAsync(IRequest request)
    {
        StringBuilder builder = new StringBuilder();

        string http = request.ShouldUseHttps() ? "https://" : "http://";

        builder.Append(http).Append(request.GetHost()).Append(request.GetRelativeUrl());

        var urlParameters = request.GetUrlParameters();

        // If we have no parameters to pass through, create a new list
        if (urlParameters == null)
        {
            urlParameters = new List<string>();
        }

        // Add Api Key
        urlParameters.Add($"key={_apiKey}");

        // Start of parameters
        builder.Append("?");

        // Add each non null or white spaced parameter and aggregate them together with a & in-between each parameter
        builder.Append(urlParameters.Where(arg => !string.IsNullOrWhiteSpace(arg))
            .Aggregate(string.Empty, (current, arg) => current + "&" + arg));


        // Create the request message with our intended http method and our built url
        var requestMessage = new HttpRequestMessage(request.GetHttpMethod(), builder.ToString());

        // add any passed in header parameters
        var headerParameters = request.GetHeaderParameters();
        if (headerParameters != null)
        {
            foreach (KeyValuePair<string, string> pair in headerParameters)
            {
                requestMessage.Headers.Add(pair.Key, pair.Value);
            }
        }

        // Add our ETag if it was in the request
        if (request.HasETag)
        {
            // ETags needs to be added without validation for now as it throws a formatting error whenever added like any other header
            requestMessage.Headers.TryAddWithoutValidation("If-None-Match", request.ETag);
        }

        // Add header data saying we want our return type to be an application/json value
        requestMessage.Headers.Add("Accept", "application/json");

        return requestMessage;
    }

    public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage requestMessage)
    {
        var response = await _client.SendAsync(requestMessage);
        /* Make sure our response code is either successful or that the content was not modified,
           Status code 304 Not Modified will still return a failure status code */
        if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NotModified)
        {
            throw new YoutubeInvalidRequestException($"Youtube Request failed.\n" +
                                                     $"Status Code: {response.StatusCode}\n" +
                                                     $"Error Reason: {response.ReasonPhrase}");
        }

        return response;
    }
}