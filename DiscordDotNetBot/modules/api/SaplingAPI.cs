using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Discord_CSharp_Bot.modules.api
{
    public struct SaplingResponseData
    {
        private string _originalMessage;
        private string? _modifiedMessage;
        private bool _isContentModified;
        private HttpResponseMessage? _httpResponseMessage;

        public string OriginalMessage { get => _originalMessage; set => _originalMessage = value; }
        public string? ModifiedMessage
        {
            get => this._modifiedMessage;
            set
            {
                this._modifiedMessage = value;
                IsContentModified = _modifiedMessage != null && !_originalMessage.Equals(_modifiedMessage);
            }
        }
        public bool IsContentModified { get => _isContentModified; set => _isContentModified = value; }
        public HttpResponseMessage? ResponseMessage { get => _httpResponseMessage; set => _httpResponseMessage = value; }

        public SaplingResponseData(HttpResponseMessage? httpResponseMessage, string originalMessage)
        {
            _httpResponseMessage = httpResponseMessage;
            _originalMessage = originalMessage;
            _modifiedMessage = null;
            _isContentModified = false;
        }
    }
    
    public class SaplingRequestContent
    {
        public SaplingRequestContent(string apiToken, string content, string sessionId = "DefaultSessionId")
        {
            ApiToken = apiToken;
            Content = content;
            SessionId = sessionId;
        }

        [JsonProperty("key")]
        public string ApiToken { get; set; }
        
        [JsonProperty("text")]
        public string Content { get; set; }
        
        [JsonProperty("session_id")]
        public string SessionId { get; set; }
    }
    
    public class SaplingApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;

        public SaplingApiClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _token = configuration["SaplingApiKey"] ?? throw new InvalidOperationException($"{this.ToString()} failed to find YoutubeApiKey in configuration service!");
        }

        public async Task<SaplingResponseData> RequestGrammarCorrectionAsync(string message)
        {
            var json = CreateRequestJson(message);
            var response = await _httpClient.PostAsync(
                "https://api.sapling.ai/api/v1/edits",
                new StringContent(json, Encoding.UTF8, "application/json"));

            SaplingResponseData saplingResponseData = new SaplingResponseData(response, message);

            JObject? jsonObject = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync());
            if (jsonObject == null)
            {
                return saplingResponseData;
            }

            saplingResponseData.ModifiedMessage = ParseGrammarResponseData(jsonObject, message);
            return saplingResponseData;
        }

        private string ParseGrammarResponseData(JObject jObject, string originalMessage)
        {
            JArray? array = jObject["edits"]?.Value<JArray>();
            if (array == null || !array.HasValues)
            {
                return originalMessage;
            }

            StringBuilder newString = new StringBuilder(originalMessage);
            
            var startPositions = new List<int>();
            var endPositions = new List<int>();
            var corrections = new List<string>();

            // Cache each starting position, end position, correct word and mistake for each of our tokens
            foreach (var token in array)
            {
                var startPosition = (token["start"] ?? throw new InvalidOperationException()).Value<int>();
                var endPosition = (token["end"] ?? throw new InvalidOperationException()).Value<int>();
                var replacement = (token["replacement"] ?? throw new InvalidOperationException()).Value<string>();
                
                startPositions.Add(startPosition);
                endPositions.Add(endPosition);
                corrections.Add(replacement);
            }

            // How much our starting and end positions have changed from other grammar/spelling edits
            int offset = 0;
            
            for (int i = 0; i < startPositions.Count; ++i)
            {
                // Get our starting and end positions based on the original value + the total offset from modifying the string
                int start = startPositions[i] + offset;
                int end = endPositions[i] + offset;
                int mistakeLength = end - start;

                newString.Remove(start, mistakeLength).Insert(start, corrections[i]);
                offset += corrections[i].Length - mistakeLength;
            }

            return newString.ToString();
        }
        private string CreateRequestJson(string message)
        {
            return JsonConvert.SerializeObject(new SaplingRequestContent(_token, message));
        }
    }
}