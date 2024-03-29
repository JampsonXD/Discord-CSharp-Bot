﻿using ClientService.ClientService;
using ClientService.ServiceRequests;
using Newtonsoft.Json.Linq;
using SaplingClient.Models;

namespace SaplingClient.Requests;

public class GrammarCheckingServiceRequest: BaseServiceRequest<SaplingGrammarResponse>
{
    public override HttpMethod HttpMethod => HttpMethod.Post;
    
    public string? Message { get; set; }

    public GrammarCheckingServiceRequest(IClientService service) : base(service)
    {
        
    }
    
    public override string RelativePath => "edits";

    protected override object? GetBody()
    {
        // Let our ClientService handle invalid requests if our body is not set
        if (Message == null)
        {
            return null;
        }
        
        return new SaplingRequestContent()
        {
            ApiToken = ClientService.ApiKey,
            Content = Message,
            SessionId = "Default"
        };
    }
}