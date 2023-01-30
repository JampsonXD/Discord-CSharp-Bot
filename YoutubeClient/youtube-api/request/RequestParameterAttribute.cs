namespace YoutubeClient.youtube_api.request;

[AttributeUsage(AttributeTargets.Property)]
public class RequestParameter: System.Attribute
{
    public string ParameterName { get; }
    public bool IsRequired { get; }

    public RequestParameter(string parameterName, bool isRequired = false)
    {
        ParameterName = parameterName;
        IsRequired = isRequired;
    }
}