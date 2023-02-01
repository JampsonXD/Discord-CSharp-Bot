namespace ClientService.Core.Authorization;

public interface IAuthorizer
{
    public Task<OAuthTokenResponse> GetTokenAsync(CancellationToken cancellationToken);
}