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


using System.Text;
using System.Text.Json;


namespace snapwonders_csharp_client
{


public class ApiJobs
{
    private static readonly HttpClient httpClient = new();


    //  Create an analyse job and display results
    public static void AnalyseJob(string pathFileName)
    {

        //  Create upload media url and upload file in data chunks
        string urlUploadMedia = ApiUpload.CreateUploadMediaUrl(pathFileName);
        ApiUpload.UploadMedia(urlUploadMedia, pathFileName);


        //  Create an analyse job url
        string urlJobStatus = CreateAnalyseJob(urlUploadMedia);

        //  Track the job status, wait until analyse job is completed
        string urlJobResults = String.Empty;
        while (true)
        {
            JobStatus jsonJobStatus = GetJobStatus(urlJobStatus);

            //  Wait for the job to be completed
            if (jsonJobStatus.status.Equals(ApiHelper.JOB_STATUS_WAITING)
                || jsonJobStatus.status.Equals(ApiHelper.JOB_STATUS_PROCESSING))
            {
                Console.WriteLine("INFO: Sleeping for a few seconds...");
                Thread.Sleep(5000);
            }
            //  If completed we break out
            else if (jsonJobStatus.status.Equals(ApiHelper.JOB_STATUS_COMPLETED))
            {
                urlJobResults = jsonJobStatus.resultUrl;
                break;
            }
            //  Some unknown state?
            else
            {
                LogHelper.LogAndExit(string.Format("ERROR: Analyse job failed with status:[{0}], message:[{1}]",
                                                   jsonJobStatus.status,
                                                   jsonJobStatus.message));
            }
        }


        //  Get and display results
        string dataResults = GetJobResults(urlJobResults);
        Console.WriteLine(JsonPrettify(dataResults));
    }


    //  Create an analyse job
    private static string CreateAnalyseJob(string urlUploadMedia)
    {
        Console.WriteLine("CALL: createAnalyseJob()");

        //  Build up Json Analyse Job
        JobAnalyse jobAnalyse = new()
        {
            key = Path.GetFileName(urlUploadMedia),
            enableTips = true,
            enableExtraAnalysis = true
        };
        string jsonJobAnalyse = JsonSerializer.Serialize(jobAnalyse);


        //  Call API to create the media url for uploading
        HttpRequestMessage requestMessage = new(HttpMethod.Post,
                                                ApiHelper.URL_SNAPWONDERS_API + ApiHelper.URL_JOB_CREATE_ANALYSE)
        {
            Content = new StringContent(jsonJobAnalyse, Encoding.UTF8, ApiHelper.HTTP_CONTENT_TYPE_JSON)
        };
        ApiHelper.AddApiHeaders(ref requestMessage);

        Task<HttpResponseMessage> result = Task.Run(async () => await httpClient.SendAsync(requestMessage));
        HttpResponseMessage response = result.Result;

        Task<string> resultContent = Task.Run(response.Content.ReadAsStringAsync);
        string jsonContent = resultContent.Result;


        //  Check POST status for errors
        if (!response.IsSuccessStatusCode)
        {
            LogHelper.LogAndExit(string.Format("ERROR: Send POST request failed:[{0}]", jsonContent));
        }

        //  Success - Extract the media url
        string urlJobAnalyse = string.Empty;
        IEnumerable<string> headers;
        if (response.Headers.TryGetValues("Location", out headers))
        {
            urlJobAnalyse = headers.First();
        }
        if (urlJobAnalyse.Length <= 0)
        {
            LogHelper.LogAndExit("ERROR: Job URL extraction failed");
        }

        Console.WriteLine(string.Format("SUCCESS: Created analyse job located at url:[{0}]", urlJobAnalyse));
        return urlJobAnalyse;
    }


    //  Gets the job status
    private static JobStatus GetJobStatus(string urlJobStatus)
    {
        Console.WriteLine("CALL: getJobStatus()");

        HttpRequestMessage requestMessage = new(HttpMethod.Post, urlJobStatus);
        ApiHelper.AddApiHeaders(ref requestMessage);

        Task<HttpResponseMessage> result = Task.Run(async () => await httpClient.SendAsync(requestMessage));
        HttpResponseMessage response = result.Result;

        Task<string> resultContent = Task.Run(response.Content.ReadAsStringAsync);
        string jsonContent = resultContent.Result;


        //  Check POST status for errors
        if (!response.IsSuccessStatusCode)
        {
            LogHelper.LogAndExit(string.Format("Create analyse job failed:[{0}]", jsonContent));
        }

        JobStatus jobStatus = JsonSerializer.Deserialize<JobStatus>(jsonContent);
        Console.WriteLine(string.Format("SUCCESS: Have job status:[{0}]", jobStatus.status));

        return jobStatus;
    }


    //  Gets the job results
    private static string GetJobResults(string urlJobResults)
    {
        Console.WriteLine("CALL: getJobResult()");

        HttpRequestMessage requestMessage = new(HttpMethod.Get, urlJobResults);
        ApiHelper.AddApiHeaders(ref requestMessage);

        Task<HttpResponseMessage> result = Task.Run(async () => await httpClient.SendAsync(requestMessage));
        HttpResponseMessage response = result.Result;

        Task<string> resultContent = Task.Run(response.Content.ReadAsStringAsync);
        string jsonContent = resultContent.Result;


        //  Check POST status for errors
        if (!response.IsSuccessStatusCode)
        {
            LogHelper.LogAndExit(string.Format("Create analyse job failed:[{0}]", jsonContent));
        }

        return jsonContent;
    }


    //  Json prettify a JSON unknown object
    private static string JsonPrettify(string json)
    {
        using var jDoc = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });
    }
}


}