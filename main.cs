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


//  NOTE: see constants_helper.cs for constants you MUST setup


Console.WriteLine("snapWONDERS Client OpenAPI v3 C# Example!");
Console.WriteLine("You must set your API key and media path/filename - see constants_helper.cs");


//  Create an analyse job and display results
snapwonders_csharp_client.ApiJobs.AnalyseJob(snapwonders_csharp_client.ConstHelper.MEDIA_PATH_FILENAME);