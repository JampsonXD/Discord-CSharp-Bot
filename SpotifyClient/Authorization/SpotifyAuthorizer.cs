using ClientService.Core.Authorization;
using Microsoft.Extensions.Options;

namespace SpotifyClient.Authorization;

public class SpotifyAuthorizer: Authorizer
{

    public SpotifyAuthorizer(IAuthorizationConfigurationOptions options) : base(options)
    {
        
    }

    public SpotifyAuthorizer(IOptions<SpotifyAuthorizationOptions> options): base(options.Value)
    {
        
    }
}