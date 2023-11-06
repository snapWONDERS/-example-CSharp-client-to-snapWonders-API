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


//  The Analyse preferences
[Serializable]
public class JobAnalyse
{
    public required string key {get; set;}
    public required bool enableTips {get; set;}
    public required bool enableExtraAnalysis {get; set;}
}


}