﻿namespace ClientService.ServiceRequests;

[AttributeUsage(AttributeTargets.Property)]
public class RequestQueryParameter: Attribute
{
    public string ParameterName { get; }
    public bool IsRequired { get; }

    public RequestQueryParameter(string parameterName, bool isRequired = false)
    {
        ParameterName = parameterName;
        IsRequired = isRequired;
    }
}