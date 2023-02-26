using System.Collections;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using ClientService.ClientService;
using ClientService.Core.Validation;
using ClientService.ServiceRequests.Interfaces;
using Newtonsoft.Json;

namespace ClientService.ServiceRequests;

/// <summary>
/// Base Class for a Service Request. Contains functionality for handling parameter attributes via reflection,
/// default implementation for executing a service request, and initialization of child class Uri parameters.
/// </summary>

/// <seealso cref="IServiceRequest{TResponse}"/>

public abstract class BaseServiceRequest<TResponse> : IServiceRequest<TResponse>
{
    /// <summary>
    /// The HttpMethod of the HttpRequest that will be created.
    /// </summary>
    public abstract HttpMethod HttpMethod { get;}

    /// <summary>
    /// A collection of query parameters that will be added to the end of the request.
    /// </summary>
    public IList<string> UriParameters => GetUriParameters();

    /// <summary>
    /// The path of the request relative to the client service's base path.
    /// </summary>
    public abstract string RelativePath { get; }
    
    /// <summary>
    /// The Client Service relating to this request.
    /// </summary>
    public IClientService ClientService { get; }

    /// <summary>
    /// A list of cached Property Info for getting query parameters via reflection.
    /// </summary>
    private readonly List<PropertyInfo> _requestParameterProperties;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseServiceRequest"/> class.
    /// </summary>
    /// <param name="service">The client service this request relates to.</param>
    protected BaseServiceRequest(IClientService service)
    {
        ClientService = service;
        _requestParameterProperties = new List<PropertyInfo>();
        InitializeRequestParameterProperties();
    }
    
    /// <summary>
    /// Executes the service request. Calls <see cref="ExecuteRequestAsync"/> and awaits the result.
    /// </summary>
    /// <returns>The</returns>
    public TResponse ExecuteRequest()
    {
        return ExecuteRequestAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Executes the service request.
    /// </summary>
    /// <returns>The response object.</returns>
    public async Task<TResponse> ExecuteRequestAsync()
    {
        using var message = CreateRequestMessage();
        var response = await ClientService.HttpClient.SendAsync(message);
        if (response.IsSuccessStatusCode)
        {
            return ClientService.HandleHttpResponseMessage<TResponse>(response);
        }
        
        HandleErrorResponse(response);
        return ClientService.HandleHttpResponseMessage<TResponse>(response);
    }

    /// <summary>
    /// Virtual method allowing child classes to handle any errors that occured from sending and receiving our request.
    /// </summary>
    /// <param name="response">The <see cref="HttpResponseMessage"/> returned from our request.</param>
    protected virtual void HandleErrorResponse(HttpResponseMessage response)
    {
        
    }

    /// <summary>
    /// Virtual method allowing child classes to add a body object to send with the request.
    /// </summary>
    /// <returns>The body object.</returns>
    protected virtual object? GetBody()
    {
        return null;
    }

    /// <summary>
    /// Gets all query parameters as a list of <see cref="string"/>s.
    /// </summary>
    /// <returns>The list of query parameters.</returns>
    private IList<string> GetUriParameters()
    {
        var list = new List<string>();
        AddStaticUriParameters(list);
        AddUriParametersFromParameterAttributes(list);
        return list;
    }

    /// <summary>
    /// Virtual method allowing any non-reflection based query parameters to be added.
    /// </summary>
    /// <param name="parametersList">The parameter list for new query parameters to be added to.</param>
    protected virtual void AddStaticUriParameters(IList<string> parametersList)
    {
        
    }
    
    /// <summary>
    /// Generates a Request Uri as a string to be used with our request.
    /// </summary>
    /// <returns>The URI string.</returns>
    string GenerateRequestUri()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(ClientService.BaseUri).Append(RelativePath);
        // Cache our parameters so we don't need to recalculate
        var uriParams = UriParameters;
        if (uriParams.Count > 0)
        {
            builder.Append("?");
            builder.Append(uriParams.Where(val => !string.IsNullOrWhiteSpace(val))
                .Aggregate(string.Empty, (current, next) => current + "&" + next));
        }

        return builder.ToString();
    }

    /// <summary>
    /// Creates the http request message.
    /// </summary>
    /// <returns>The created http request message.</returns>
    HttpRequestMessage CreateRequestMessage()
    {
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod, GenerateRequestUri());

        object? body = GetBody();
        if (body == null) return message;
        
        var json = JsonConvert.SerializeObject(body);
        var content = new ByteArrayContent(Encoding.UTF8.GetBytes(json));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        message.Content = content;

        return message;
    }

    /// <summary>
    /// Initializes the query parameter properties. Caches all <see cref="RequestQueryParameterAttribute"/> attributes for use during request execution.
    /// </summary>
    void InitializeRequestParameterProperties()
    {
        _requestParameterProperties.Clear();
        _requestParameterProperties.InsertRange(0, 
            this.GetType().GetProperties().Where(prop => prop.GetCustomAttribute<RequestQueryParameterAttribute>() != null));
    }

    /// <summary>
    /// Creates a query string to be added to <see cref="UriParameters"/> from a <see cref="RequestQueryParameterAttribute"/> and its associated value.
    /// </summary>
    /// <param name="attribute">The attribute instance.</param>
    /// <param name="propertyValue">The properties value.</param>
    /// <returns>A query string.</returns>
    string GetQueryStringFromParameterAttribute(RequestQueryParameterAttribute attribute, object? propertyValue)
    {
        if (propertyValue != null)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(attribute.ParameterName).Append("=");
            
            if (propertyValue is ICollection collection)
            {
                foreach (var item in collection)
                {
                    builder.Append(item).Append(',');
                }

                builder.Remove(builder.Length - 1, 1);
            }
            else
            {
                builder.Append(propertyValue);
            }

            return builder.ToString();
        }

        return string.Empty;
    }
    
    /// <summary>
    /// Adds and validates all request parameter properties for our request through reflection.
    /// Query strings are created for each individual property and then added to the query parameter list.
    /// </summary>
    /// <param name="list">The query parameter list.</param>
    /// <exception cref="Exception">Thrown when a property is invalid and does not meet the attributes criteria.</exception>
    void AddUriParametersFromParameterAttributes(IList<string> list)
    {
        foreach (var prop in _requestParameterProperties)
        {
            // These requests have already been null checked and verified to have a Request Parameter Attribute when first initialized
            var attribute = prop.GetCustomAttribute<RequestQueryParameterAttribute>();
            Debug.Assert(attribute != null);
            
            // Get any attribute validators the property might have been tagged with and use these for validation
            var attributeValidators = prop.GetCustomAttributes<PropertyValidatorAttribute>();

            var value = prop.GetValue(this);
            
            // Thrown an error if our parameter value does not meet all of the validator requirements
            if (!attributeValidators.All(validator => validator.IsValid(value)))
            {
                // TODO: Create an exception specifically for requests and add extra information
                throw new Exception();
            }
            
            var parameterValue = GetQueryStringFromParameterAttribute(attribute, value);
            if (!string.IsNullOrWhiteSpace(parameterValue))
            {
                list.Add(parameterValue);
            }
        }
    }
}