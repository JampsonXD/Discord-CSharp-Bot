using System.Net;
using System.Net.Http.Headers;
using ClientService.Core.Authorization;

namespace ClientService.Core;

/// <summary>
/// Http Handler class for injecting authorization header information into http request messages.
/// Handler gets token information from an Authorizer, and handles requesting a new token on expiration.
/// </summary>

/// <seealso cref="DelegatingHandler"/>

public class OAuthHttpHandler: DelegatingHandler
{
    /// <summary>
    /// The authorizer to retrieve the authentication token from.
    /// </summary>
    private readonly IAuthorizer _authorizer;
    /// <summary>
    /// Cached token response data.
    /// </summary>
    private OAuthTokenResponse? _tokenResponse;
    

    /// <summary>
    /// Initializes a new instance of the <see cref="OAuthHttpHandler"/> class
    /// </summary>
    /// <param name="authorizer">The authorizer to retrieve token information from.</param>
    /// <param name="innerHandler">Inner handler that processes the http response message.</param>
    public OAuthHttpHandler(IAuthorizer authorizer, HttpMessageHandler? innerHandler = null)
    {
        _authorizer = authorizer;
        _tokenResponse = null;
        InnerHandler = innerHandler ?? new HttpClientHandler();
    }

    /// <summary>
    /// Retrieves the token information, adds it as an authentication header, and passes on the request.
    /// If the response comes back as Unauthorized, the token is refreshed and the request is sent again.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequestMessage"/> to send to the server.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel operation.</param>
    /// <returns>The response</returns>
    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return SendAsync(request, cancellationToken).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Gets the token response data to add to our header.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel operation.</param>
    /// <returns>OAuth token information needed for an authentication header.</returns>
    private async Task<OAuthTokenResponse> GetTokenResponseAsync(CancellationToken cancellationToken)
    {
        return await _authorizer.GetTokenAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves the token information, adds it as an authentication header, and passes on the request.
    /// If the response comes back as Unauthorized, the token is refreshed and the request is sent again.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequestMessage"/> to send to the server.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to cancel operation.</param>
    /// <returns>The response</returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _tokenResponse ??= await GetTokenResponseAsync(cancellationToken);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(_tokenResponse.TokenType, _tokenResponse.AccessToken);

        var response = await base.SendAsync(request, cancellationToken);
        
        /* Request a new access token if the current token has expired and add the response data as our authorization header */
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _tokenResponse = await GetTokenResponseAsync(cancellationToken);
            request.Headers.Authorization =
                new AuthenticationHeaderValue(_tokenResponse.TokenType, _tokenResponse.AccessToken);
            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}