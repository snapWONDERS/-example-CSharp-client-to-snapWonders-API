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


public class FileHelper
{

    //  Determine total file upload size
    public static long DetermineFileSize(string fileName)
    {
        long fileTotalSize = -1;
        try
        {
            FileInfo fileStat = new(fileName);

            fileTotalSize = fileStat.Length;
            if (fileTotalSize <= 0)
            {
                LogHelper.LogAndExit(string.Format("ERROR: Illegal file size:[{0}]", fileTotalSize));
            }
        }
        catch(Exception ex)
        {
            LogHelper.LogAndExit(string.Format("ERROR: Failed to stat file:[{0}]", ex.Message));
        }

        return fileTotalSize;
    }
}


}