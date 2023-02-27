using ClientService.Core.Validation;
using ClientService.ServiceRequests.Interfaces;

namespace ClientService.ServiceRequests;

/// <summary>
/// Represents a custom attribute for creating a query string for a property value via reflection.
/// </summary>

/// <seealso cref="Attribute"/>

[AttributeUsage(AttributeTargets.Property)]
public class QueryParameterAttribute: Attribute
{
    /// <summary>
    /// The query parameter name.
    /// </summary>
    /// <example>
    /// ParameterName = "id", PropertyValue = "123" would create a query string "id=123".
    /// </example>
    public string ParameterName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryParameterAttribute"/> class.
    /// </summary>
    /// <param name="parameterName">The parameters name.</param>
    public QueryParameterAttribute(string parameterName)
    {
        ParameterName = parameterName;
    }
}