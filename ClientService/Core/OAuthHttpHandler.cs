using System.Net;
using System.Net.Http.Headers;
using ClientService.Core.Authorization;

namespace ClientService.Core;

/* Http Handler class for injecting authorization header information into http request messages.
   Handler gets token information from an Authorizer, and handles requesting a new token on expiration.
 */
public class OAuthHttpHandler: DelegatingHandler
{
    private readonly IAuthorizer _authorizer;
    private OAuthTokenResponse? _tokenResponse;
    

    public OAuthHttpHandler(IAuthorizer authorizer, HttpMessageHandler? innerHandler = null)
    {
        _authorizer = authorizer;
        _tokenResponse = null;
        InnerHandler = innerHandler ?? new HttpClientHandler();
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {

        _tokenResponse ??= GetTokenResponseAsync(cancellationToken).GetAwaiter().GetResult();
        request.Headers.Authorization =
            new AuthenticationHeaderValue(_tokenResponse.TokenType, _tokenResponse.AccessToken);

        var response = base.Send(request, cancellationToken);
        
        /* Request a new access token if the current token has expired and add the response data as our authorization header */
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _tokenResponse = GetTokenResponseAsync(cancellationToken).GetAwaiter().GetResult();
            request.Headers.Authorization =
                new AuthenticationHeaderValue(_tokenResponse.TokenType, _tokenResponse.AccessToken);
            response = base.Send(request, cancellationToken);
        }

        return response;
    }

    private async Task<OAuthTokenResponse> GetTokenResponseAsync(CancellationToken cancellationToken)
    {
        return await _authorizer.GetTokenAsync(cancellationToken);
    }

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