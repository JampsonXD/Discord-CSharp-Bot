using System.Text;
using SaplingClient.Models;
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

        private static string ParseGrammarResponseData(SaplingGrammarResponse response, string originalMessage)
        {
            if (response.Edits.Count < 1)
            {
                return originalMessage;
            }

            StringBuilder newString = new StringBuilder(originalMessage);

            // How much our starting and end positions have changed from other grammar/spelling edits
            int offset = 0;
            
            foreach (var edit in response.Edits)
            {
                // Get our starting and end positions based on the original value + the total offset from modifying the string
                int start = edit.Start + offset;
                int end = edit.End + offset;
                int mistakeLength = end - start;

                newString.Remove(start, mistakeLength).Insert(start, edit.Replacement);
                offset += edit.Replacement.Length - mistakeLength;
            }

            return newString.ToString();
        }
    }
}