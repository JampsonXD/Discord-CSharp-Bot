namespace YoutubeClient.youtube_api.exceptions;

[Serializable]
public class YoutubeInvalidRequestException : Exception
{
    public YoutubeInvalidRequestException() : base() { }
    public YoutubeInvalidRequestException(string message, Exception inner) : base(message, inner) { }
    public YoutubeInvalidRequestException(string message) : base(message) { }
}