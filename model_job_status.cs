/*
 * snapWONDERS OpenAPI Specification
 * API version: 1.0
 *
 * Copyright (c) snapWONDERS.com, All rights reserved 2023
 *
 * Author: Kenneth Springer (https://kennethbspringer.au)
 *
 * All the snapWONDERS API services is available over the Clearnet / **Web** and Dark Web **Tor** and **I2P**
 * Read details: https://snapwonders.com/snapwonders-openapi-specification
 *
 */


namespace snapwonders_csharp_client
{


//  The job status and result-url
[Serializable]
public class JobStatus
{
    public required string key {get; set;}
    public required string status {get; set;}
    public required string message {get; set;}
    public required string resultUrl {get; set;}
}


}