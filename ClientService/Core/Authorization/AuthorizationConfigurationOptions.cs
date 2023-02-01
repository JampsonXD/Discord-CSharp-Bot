namespace ClientService.Core.Authorization;

public class AuthorizationConfigurationOptions
{
    public Uri TokenRequestUri { get; set; }
    public string ClientSecret { get; set; }
    public string ClientId { get; set; }
}