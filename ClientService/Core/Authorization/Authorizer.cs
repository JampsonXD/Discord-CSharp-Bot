using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace ClientService.Core.Authorization;

/// <summary>
/// Represents a basic OAuth authorization request to retrieve authorization information using a REST API.
/// </summary>

/// <seealso cref="IAuthorizer"/>

public class Authorizer: IAuthorizer
{
    /// <summary>
    /// Configuration options.
    /// </summary>
    private readonly IAuthorizationConfigurationOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="Authorizer"/> class.
    /// </summary>
    /// <param name="options">Configuration options to use when making the request.</param>
    public Authorizer(IAuthorizationConfigurationOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Gets the token using the specified cancellation token
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel operations.</param>
    /// <exception cref="Exception">An exception thrown if there is any problems retrieving or deserializing the authentication token response.</exception>
    /// <returns>OAuth token information.</returns>
    public async Task<OAuthTokenResponse> GetTokenAsync(CancellationToken cancellationToken)
    {
        using var request = CreateRequest();
        using var client = new HttpClient();
        var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception();
        }
            
        return JsonConvert.DeserializeObject<OAuthTokenResponse>(await response.Content.ReadAsStringAsync(cancellationToken))
               ?? throw new Exception();
    }

    /// <summary>
    /// Creates a http request message used to retrieve the authorization token response data.
    /// </summary>
    /// <returns>The configured <see cref="HttpRequestMessage"/>.</returns>
    private HttpRequestMessage CreateRequest()
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _options.TokenRequestUri);
        request.Headers.Authorization =
            new AuthenticationHeaderValue("Basic", EncodeMessage(_options.ClientId + ":" + _options.ClientSecret));

        var values = new Dictionary<string, string>()
        {
            {"grant_type", "client_credentials"}
        };
        
        request.Content = new FormUrlEncodedContent(values);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return request;
    }

    /// <summary>
    /// Encodes a message string to Base64
    /// </summary>
    /// <param name="message">The message to be encoded.</param>
    /// <returns>The encoded message.</returns>
    private string EncodeMessage(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        return Convert.ToBase64String(bytes);
    }
}