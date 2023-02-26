namespace ClientService.Core.Authorization;

/// <summary>
/// Represents configuration options needed to make an authorization request.
/// </summary>

public interface IAuthorizationConfigurationOptions
{
    /// <summary>
    /// The URI to make the token request.
    /// </summary>
    public Uri TokenRequestUri { get;}
    /// <summary>
    /// The client secret needed for the authorization request.
    /// </summary>
    public string ClientSecret { get;}
    /// <summary>
    /// The client id needed for the authorization request.
    /// </summary>
    public string ClientId { get;}
}