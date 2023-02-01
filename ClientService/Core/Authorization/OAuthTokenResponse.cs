using Newtonsoft.Json;

namespace ClientService.Core.Authorization;

public class OAuthTokenResponse
{
    internal OAuthTokenResponse()
    {
        
    }
    
    [JsonProperty("access_token")] 
    public string AccessToken { get; set; }

    [JsonProperty("token_type")]
    public string TokenType { get; set; }
    
    [JsonProperty("expires_in")]
    public int Expiration { get; set; }

}