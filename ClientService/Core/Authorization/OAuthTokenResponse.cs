using Newtonsoft.Json;

namespace ClientService.Core.Authorization;

/// <summary>
/// Represents OAuth response data from an authorization request.
/// </summary>

public class OAuthTokenResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OAuthTokenResponse"/> class
    /// </summary>
    internal OAuthTokenResponse()
    {
        
    }
    
    /// <summary>
    /// The access token returned from the authorization request.
    /// </summary>
    [JsonProperty("access_token")] 
    public required string AccessToken { get; set; }

    /// <summary>
    /// The type of access token returned from the authorization request.
    /// </summary>
    [JsonProperty("token_type")]
    public required string TokenType { get; set; }
    
    /// <summary>
    /// How long the access token has in seconds before it expires.
    /// </summary>
    [JsonProperty("expires_in")]
    public int Expiration { get; set; }

}