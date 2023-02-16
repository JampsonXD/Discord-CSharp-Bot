using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace ClientService.Core.Authorization;

public class Authorizer: IAuthorizer
{
    private readonly IAuthorizationConfigurationOptions _options;

    public Authorizer(IAuthorizationConfigurationOptions options)
    {
        _options = options;
    }

    public async Task<OAuthTokenResponse> GetTokenAsync(CancellationToken cancellationToken)
    {
        var request = CreateRequest();
        using (var client = new HttpClient())
        {
            var response = await client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception();
            }
            
            return JsonConvert.DeserializeObject<OAuthTokenResponse>(await response.Content.ReadAsStringAsync(cancellationToken))
                ?? throw new Exception();
        }
    }

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

    private string EncodeMessage(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        return Convert.ToBase64String(bytes);
    }
}