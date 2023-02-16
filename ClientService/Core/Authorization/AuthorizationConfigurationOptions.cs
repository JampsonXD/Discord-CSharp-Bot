namespace ClientService.Core.Authorization;

public interface IAuthorizationConfigurationOptions
{
    public Uri TokenRequestUri { get;}
    public string ClientSecret { get;}
    public string ClientId { get;}
}