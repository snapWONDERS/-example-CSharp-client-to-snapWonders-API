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


using System.Text.Json;


namespace snapwonders_csharp_client
{


public class ApiUpload
{
    private static readonly HttpClient httpClient = new();


    //  Create an upload media URL
    public static string CreateUploadMediaUrl(string mediaPathFileName)
    {
        Console.WriteLine("CALL: createUploadMediaUrl()");

        //  Build up Json File Metadata
        FileMetadata fileMetadata = new()
        {
            name = Path.GetFileName(mediaPathFileName),
            size = FileHelper.DetermineFileSize(mediaPathFileName)
        };
        string jsonfileMetadata = JsonSerializer.Serialize(fileMetadata);


        //  Call API to create the media url for uploading
        HttpRequestMessage requestMessage = new(HttpMethod.Post,
                                                ApiHelper.URL_SNAPWONDERS_API + ApiHelper.URL_UPLOAD_CREATE_MEDIA_URL)
        {
            Content = new StringContent(jsonfileMetadata, System.Text.Encoding.UTF8, ApiHelper.HTTP_CONTENT_TYPE_JSON)
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
        string urlCreateMedia = string.Empty;
        IEnumerable<string> headers;
        if (response.Headers.TryGetValues("Location", out headers))
        {
            urlCreateMedia = headers.First();
        }
        if (urlCreateMedia.Length <= 0)
        {
            LogHelper.LogAndExit("ERROR: Upload media URL extraction failed");
        }

        Console.WriteLine(string.Format("SUCCESS: Created resumable uploading media url:[{0}]", urlCreateMedia));
        return urlCreateMedia;
    }


    //  Read chunks from the file stream
    private static IEnumerable<byte[]> ReadChunks(string fileName)
    {
        using FileStream fs = new(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        long remainingBytes = fs.Length;

        while (true)
        {

            //  Allocate the chunk byte storage
            int bufferBytes = ConstHelper.DATA_CHUNK_SIZE;
            if (remainingBytes <= 0)
            {
                break;
            }
            else if (remainingBytes <= ConstHelper.DATA_CHUNK_SIZE)
            {
                bufferBytes = (int)remainingBytes;
            }
            byte[] fileChunk = new byte[bufferBytes];


            //  Read from stream
            int numBytes = fs.Read(fileChunk, 0, bufferBytes);
            if (0 < numBytes)
            {

                //  Adjust byte stream if read in less so the data chunk length is correct
                if (numBytes < bufferBytes)
                {
                    Array.Resize(ref fileChunk, numBytes);
                }
                remainingBytes -= numBytes;

                yield return fileChunk;
            }
            else
            {
                break;
            }
        }
    }


    //  Uploads file to given media url
    public static void UploadMedia(string urlUploadMedia,string mediaPathFileName)
    {
        Console.WriteLine("CALL: uploadMedia()");

        int offset = 0;
        foreach (byte[] dataChunk in ReadChunks(mediaPathFileName))
        {
            offset = UploadDataChunk(urlUploadMedia, dataChunk, offset);
        }

        Console.WriteLine(string.Format("SUCCESS: Uploaded file:[{0}] to media url:[{1}]", mediaPathFileName, urlUploadMedia));
    }


    //  Uploads a data chunk
    private static int UploadDataChunk(string urlUploadMedia, byte[] dataChuck, int offset)
    {
        //  Build the multipart form data for uploading which includes the offset and chunked data
        MultipartFormDataContent multipartData = new()
        {
            { new StringContent(offset.ToString()), "offset" },
            { new StreamContent(new MemoryStream(dataChuck)), "file" }
        };


        //  Patch the data chunk for uploading to given media url
        HttpRequestMessage requestMessage = new(HttpMethod.Patch, urlUploadMedia)
        {
            Content = multipartData
        };
        ApiHelper.AddApiHeaders(ref requestMessage);

        Task<HttpResponseMessage> result = Task.Run(async () => await httpClient.SendAsync(requestMessage));
        HttpResponseMessage response = result.Result;

        Task<string> resultContent = Task.Run(response.Content.ReadAsStringAsync);
        string jsonContent = resultContent.Result;


        //  Check for upload errors.
        //  Note: If an upload failed, you can retry uploading from the last offset. Call the HEAD request to determine
        //  the last offset position if you are not sure what that is. Uploading is resumable and can be continued
        //  at a later time (which is useful if there is a network outage or connectivity issue)
        //  snapWONDERS uploading follows the Tus.io protocol
        if (!response.IsSuccessStatusCode)
        {
            LogHelper.LogAndExit(string.Format("ERROR: Send PATCH request failed:[{0}]", jsonContent));
        }

        //  Extract the upload offset
        int uploadOffset = 0;
        IEnumerable<string> headers;
        if (response.Headers.TryGetValues("Upload-Offset", out headers))
        {
            uploadOffset = int.Parse(headers.First());
        }
        return uploadOffset;
    }
}


}