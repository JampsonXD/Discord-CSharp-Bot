namespace ClientService.Core.Authorization;

/// <summary>
/// Represents an entity able to retrieve authorization information needed to access a resource.
/// </summary>

public interface IAuthorizer
{
    /// <summary>
    /// Gets an authorization token response.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> used to cancel the operation.</param>
    /// <returns>The authorization information.</returns>
    public Task<OAuthTokenResponse> GetTokenAsync(CancellationToken cancellationToken);
}