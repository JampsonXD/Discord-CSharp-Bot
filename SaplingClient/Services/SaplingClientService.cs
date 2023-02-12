using ClientService.ClientService;
using Newtonsoft.Json;
using SaplingClient.Resources;

namespace SaplingClient.Services;

public class SaplingClientService: IClientService
{
    public string ApiKey { get; }
    public string ServiceName { get; }
    public string BaseUri => "https://api.sapling.ai/api/v1/";
    public HttpClient HttpClient { get; }
    public GrammarCorrectionResource GrammarCorrectionResource { get; }

    public SaplingClientService(ClientServiceInitializer initializer)
    {
        ApiKey = initializer.ApiKey ?? throw new InvalidOperationException();
        ServiceName = initializer.ServiceName ?? "Sapling Client Service";
        HttpClient = initializer.HttpClient ?? new HttpClient();
        GrammarCorrectionResource = new GrammarCorrectionResource(this);
    }
    public T HandleHttpResponseMessage<T>(HttpResponseMessage responseMessage)
    {
        return JsonConvert.DeserializeObject<T>(responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult());
    }
}