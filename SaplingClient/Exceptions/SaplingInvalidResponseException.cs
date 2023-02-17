namespace SaplingClient.Exceptions;

public class SaplingInvalidResponseException: Exception
{
    public SaplingInvalidResponseException() : base() { }
    public SaplingInvalidResponseException(string message, Exception inner) : base(message, inner) { }
    public SaplingInvalidResponseException(string message) : base(message) { }
}