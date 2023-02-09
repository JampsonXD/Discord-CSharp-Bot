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
    public HttpMethod HttpMethod { get; protected set; }
    public IList<string> UriParameters { get; private set; }
    public abstract string RelativePath { get; }
    public IClientService ClientService { get; }

    private readonly List<PropertyInfo> _requestParameterProperties;
    

    public T ExecuteRequest()
    {
        AddParameterAttributes();
        HttpRequestMessage message = CreateRequestMessage();
        var response = ClientService.HttpClient.Send(message);
        return ClientService.HandleHttpResponseMessage<T>(response);
    }

    public async Task<T> ExecuteRequestAsync()
    {
        AddParameterAttributes();
        HttpRequestMessage message = CreateRequestMessage();
        var response = await ClientService.HttpClient.SendAsync(message);
        return ClientService.HandleHttpResponseMessage<T>(response);
    }

    protected BaseServiceRequest(ServiceRequestInitializer initializer)
    {
        HttpMethod = initializer.HttpMethod;
        ClientService = initializer.ClientService;
        UriParameters = new List<string>();

        _requestParameterProperties = new List<PropertyInfo>();
        InitializeRequestParameterProperties();
        InitializeUriParameters();
    }

    protected virtual object? GetBody()
    {
        return null;
    }

    protected virtual void InitializeUriParameters()
    {
        if (!string.IsNullOrWhiteSpace(ClientService.ApiKey))
        {
            UriParameters.Add($"key={ClientService.ApiKey}");
        }
    }

    /* Generates a Request Uri as a string to be used with our HttpRequest */
    string GenerateRequestUri()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(ClientService.BaseUri).Append(RelativePath);
        if (UriParameters.Count > 0)
        {
            builder.Append("?");
            builder.Append(UriParameters.Where(val => !string.IsNullOrWhiteSpace(val))
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
            var content = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(json));
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

    void AddParameterAttribute(RequestQueryParameter attribute, object? propertyValue)
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
            
            UriParameters.Add(builder.ToString());
        }
    }

    /* Wipes all items inside UriParameter that match parameter request attribute names */
    void ClearParameterAttributes()
    {
        List<string> parameterNames = new List<string>();
        _requestParameterProperties.ForEach(prop => parameterNames.Add(prop.GetCustomAttribute<RequestQueryParameter>().ParameterName));

        // Remove all parameters that contain the entire custom parameter name
         UriParameters = UriParameters.Where(parameter => parameterNames.All(name => parameter.Split('=', 2)[0] != name)).ToList();
    }

    /* Loops through all request parameter properties, validates them, and then adds them to our UriParameter list for request execution */
    void AddParameterAttributes()
    {
        ClearParameterAttributes();
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
            
            AddParameterAttribute(attribute, value);
        }
    }
}