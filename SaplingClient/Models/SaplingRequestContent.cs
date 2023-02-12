using Newtonsoft.Json;

namespace SaplingClient.Models;

public class SaplingRequestContent
{
    public SaplingRequestContent(string apiToken, string content, string sessionId = "DefaultSessionId")
    {
        ApiToken = apiToken;
        Content = content;
        SessionId = sessionId;
    }
    
    internal SaplingRequestContent(){}

    [JsonProperty("key")]
    public required string ApiToken { get; set; }
        
    [JsonProperty("text")]
    public required string Content { get; set; }
        
    [JsonProperty("session_id")]
    public required string SessionId { get; set; }
}