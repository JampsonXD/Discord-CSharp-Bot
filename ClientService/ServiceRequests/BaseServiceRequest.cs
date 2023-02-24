using System.Collections;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using ClientService.ClientService;
using ClientService.ServiceRequests.Interfaces;
using Newtonsoft.Json;

namespace ClientService.ServiceRequests;


/* Base Class for a Service Request. Contains functionality for handling parameter attributes via reflection,
    default implementation for executing a service request, and initialization of child class Uri parameters. */
public abstract class BaseServiceRequest<T> : IServiceRequest<T>
{
    public abstract HttpMethod HttpMethod { get;}

    public IList<string> UriParameters => GetUriParameters();

    public abstract string RelativePath { get; }
    public IClientService ClientService { get; }

    private readonly List<PropertyInfo> _requestParameterProperties;

    protected BaseServiceRequest(IClientService service)
    {
        ClientService = service;
        _requestParameterProperties = new List<PropertyInfo>();
        InitializeRequestParameterProperties();
    }
    
    public T ExecuteRequest()
    {
        using var message = CreateRequestMessage();
        var response = ClientService.HttpClient.Send(message);
        if (response.IsSuccessStatusCode)
        {
            return ClientService.HandleHttpResponseMessage<T>(response);
        }
        return ClientService.HandleHttpResponseMessage<T>(response);
    }

    public async Task<T> ExecuteRequestAsync()
    {
        using var message = CreateRequestMessage();
        var response = await ClientService.HttpClient.SendAsync(message);
        return ClientService.HandleHttpResponseMessage<T>(response);
    }

    protected virtual void HandleErrorResponse()
    {
        
    }

    protected virtual object? GetBody()
    {
        return null;
    }

    private IList<string> GetUriParameters()
    {
        var list = new List<string>();
        AddStaticUriParameters(list);
        AddUriParametersFromParameterAttributes(list);
        return list;
    }

    protected virtual void AddStaticUriParameters(IList<string> parametersList)
    {
        
    }

    /* Generates a Request Uri as a string to be used with our HttpRequest */
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

    HttpRequestMessage CreateRequestMessage()
    {
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod, GenerateRequestUri());

        object? body = GetBody();
        if (body != null)
        {
            var json = JsonConvert.SerializeObject(body);
            var content = new ByteArrayContent(Encoding.UTF8.GetBytes(json));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            message.Content = content;
        }
        
        return message;
    }

    void InitializeRequestParameterProperties()
    {
        _requestParameterProperties.Clear();
        _requestParameterProperties.InsertRange(0, 
            this.GetType().GetProperties().Where(prop => prop.GetCustomAttribute<RequestQueryParameter>() != null));
    }

    bool ValidateParameterAttributes(RequestQueryParameter attribute, object? propertyValue)
    {
        return !attribute.IsRequired || propertyValue != null;
    }

    string GetParameterAttribute(RequestQueryParameter attribute, object? propertyValue)
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

    /* Loops through all request parameter properties, validates them, and then adds them to our UriParameter list for request execution */
    void AddUriParametersFromParameterAttributes(IList<string> list)
    {
        foreach (var prop in _requestParameterProperties)
        {
            // These requests have already been null checked and verified to have a Request Parameter Attribute when first initialized
            var attribute = prop.GetCustomAttribute<RequestQueryParameter>();
            Debug.Assert(attribute != null);
            
            var value = prop.GetValue(this);
            
            // Thrown an error if our parameter value is invalid
            if (!ValidateParameterAttributes(attribute, value))
            {
                // TODO: Create an exception specifically for requests and add extra information
                throw new Exception();
            }
            
            var parameterValue = GetParameterAttribute(attribute, value);
            if (!string.IsNullOrWhiteSpace(parameterValue))
            {
                list.Add(parameterValue);
            }
        }
    }
}