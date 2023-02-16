using System.Text;
using Newtonsoft.Json.Linq;
using SaplingClient.Requests;

namespace DiscordBot.Modules.Sapling
{
    public struct SaplingResponseData
    {
        private string _originalMessage;
        private string? _modifiedMessage;
        private bool _isContentModified;

        public string OriginalMessage { get => _originalMessage; set => _originalMessage = value; }
        public string? ModifiedMessage
        {
            get => this._modifiedMessage;
            set
            {
                this._modifiedMessage = value;
                _isContentModified = _modifiedMessage != null && !_originalMessage.Equals(_modifiedMessage);
            }
        }
        public bool IsContentModified { get => _isContentModified; }

        public SaplingResponseData(string originalMessage)
        {
            _originalMessage = originalMessage;
            _modifiedMessage = null;
            _isContentModified = false;
        }
    }

    public static class SaplingClientHelpers
    {

        public static async Task<SaplingResponseData> ConvertSaplingRequestIntoResponseData(this GrammarCheckingServiceRequest request)
        {
            // Return early if we our response message is null
            if (request.Message == null)
            {
                return new SaplingResponseData()
                {
                    ModifiedMessage = null
                };
            }
            
            var response = await request.ExecuteRequestAsync();
            SaplingResponseData saplingResponseData = new SaplingResponseData(request.Message)
            {
                ModifiedMessage = ParseGrammarResponseData(response, request.Message)
            };

            return saplingResponseData;
        }

        private static string ParseGrammarResponseData(JObject jObject, string originalMessage)
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
    }
}