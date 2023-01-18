using System.Text;
using Discord_CSharp_Bot.modules.youtube_api.request.request_parameters.interfaces;

namespace Discord_CSharp_Bot.modules.youtube_api.request.request_parameters;

public class YoutubeRequestParameters : IRequestParameter
{
    public YoutubeRequestParameters()
    {
        Parameters = new Dictionary<string, List<string>>();
    }

    // Mapping of parameter names and the values they hold
    public Dictionary<string, List<string>> Parameters { get; private set; }

    /* Constructs a list of parameters that can be used in a youtube channel request.
        Each list item contains a string formatted in the following order:
         ParameterName=Value1,Value2,Value3,etc...
     */
    public List<string> ConstructParameters()
    {
        StringBuilder stringBuilder = new StringBuilder();
        List<string> paramList = new List<string>();
        
        Parameters.ToList().ForEach(valuePair =>
        {
            // The parameter type
            stringBuilder.Append(valuePair.Key + "=");
            
            // Append parameter values that are not null or white spaces with comma separation
            stringBuilder.Append(valuePair.Value.Where(arg => !string.IsNullOrWhiteSpace(arg))
                .Aggregate(string.Empty, (current, arg) =>
                {
                    // Don't add a comma to our first value
                    if (string.IsNullOrWhiteSpace(current))
                    {
                        return current + arg;
                    }
                    return current + ("," + arg);
                }));
            
            // Add the newly built string and clear the string builder for the next iteration
            paramList.Add(stringBuilder.ToString());
            stringBuilder.Clear();
        });

        return paramList;
    }
}