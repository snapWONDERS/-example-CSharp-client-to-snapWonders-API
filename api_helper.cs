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


public class ApiHelper
{
    //  The API URLs
    public const string URL_SNAPWONDERS_API         = "https://api.snapwonders.com/v1/";
    public const string URL_UPLOAD_CREATE_MEDIA_URL = "upload/create-media-url";
    public const string URL_JOB_CREATE_ANALYSE      = "job/analyse";

    //  Content types
    public const string HTTP_CONTENT_TYPE_JSON = "application/json";

    //  Job status
    public const string JOB_STATUS_WAITING    = "WAITING";
    public const string JOB_STATUS_PROCESSING = "PROCESSING";
    public const string JOB_STATUS_COMPLETED  = "COMPLETED";


    // Adds the headers
    public static void AddApiHeaders(ref HttpRequestMessage req)
    {
        req.Headers.Add("Accept", "*/*");
        req.Headers.Add("Api_Key", ConstHelper.SNAPWONDERS_API_KEY);
    }
}


}