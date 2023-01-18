namespace Discord_CSharp_Bot.modules.youtube_api.request.request_parameters;

public class YoutubeParameterBuilder
{
    private YoutubeRequestParameters _builderObject;

    internal YoutubeParameterBuilder()
    {
        _builderObject = new YoutubeRequestParameters();
    }
    
    public void Reset()
    {
        _builderObject = new YoutubeRequestParameters();
    }

    public YoutubeRequestParameters Build()
    {
        YoutubeRequestParameters parameters = _builderObject;
        Reset();
        return parameters;
    }
    
    /* Place various values as long as the value is not already contained in the list */
    protected void FindOrAddValue(string key, string value)
    {
        if(_builderObject.Parameters.TryGetValue(key, out var list))
        {
            // Only add values of the same type once
            if (!list.Contains(value))
            {
                list.Add(value);
            }
        }
        
        _builderObject.Parameters.Add(key, new List<string>() { value });
    }

    /* Place a value when only one value is allowed for the key */
    protected void FindOrReplaceValue(string key, string value)
    {
        if(_builderObject.Parameters.TryGetValue(key, out var list))
        {
            list.Insert(0, value);
        }
        
        _builderObject.Parameters.Add(key, new List<string>() { value });
    }
}

public class YoutubeChannelParameterBuilder : YoutubeParameterBuilder
{
    public YoutubeChannelParameterBuilder WithUsername(string username)
    {
        FindOrReplaceValue("forUsername", username);
        return this;
    }
    
    public YoutubeChannelParameterBuilder WithId(string id)
    {
        FindOrReplaceValue("id", id);
        return this;
    }

    public YoutubeChannelParameterBuilder WithAuditDetails()
    {
        FindOrAddValue("part", "auditDetails");
        return this;
    }

    public YoutubeChannelParameterBuilder WithBrandingSettings()
    {
        FindOrAddValue("part", "brandingSettings");
        return this;
    }

    public YoutubeChannelParameterBuilder WithContentDetails()
    {
        FindOrAddValue("part", "contentDetails");
        return this;
    }
    
    public YoutubeChannelParameterBuilder WithSnippet()
    {
        FindOrAddValue("part", "snippet");
        return this;
    }
}

public class YoutubePlaylistItemParameterBuilder : YoutubeParameterBuilder
{
    public YoutubePlaylistItemParameterBuilder WithId(string id)
    {
        FindOrReplaceValue("playlistId", id);
        return this;
    }

    public YoutubePlaylistItemParameterBuilder WithSnippet()
    {
        FindOrAddValue("part", "snippet");
        return this;
    }

    public YoutubePlaylistItemParameterBuilder WithMaxResults(int maxResults)
    {
        FindOrReplaceValue("maxResults", maxResults.ToString());
        return this;
    }
}