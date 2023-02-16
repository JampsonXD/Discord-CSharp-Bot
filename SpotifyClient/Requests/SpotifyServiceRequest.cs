﻿using ClientService.ServiceRequests;

namespace SpotifyClient.Requests;

public abstract class SpotifyServiceRequest<T>: BaseServiceRequest<T>
{
    protected SpotifyServiceRequest(ServiceRequestInitializer initializer) : base(initializer)
    {
        
    }
}