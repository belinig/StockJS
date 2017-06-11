using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace StockQuote.ExtensionMethods
{
    public static class Methods
    {
        public static WebResponse GetResponse(this HttpWebRequest webRequest)
        {
            HttpWebResponse webResponse = null;
            webResponse = Task.Factory.FromAsync(webRequest.BeginGetResponse(null, null), result =>
            {
                return (HttpWebResponse)webRequest.EndGetResponse(result);
            }).Result;
            return webResponse;
        }


        public static void Close(this StreamReader streamReader)
        {
            streamReader.Dispose();
        }

        public static void ContentLength (this HttpWebRequest webRequest, long contentLength)
        {
            webRequest.Headers[HttpRequestHeader.ContentLength] = contentLength.ToString();
        }

        
    }

}

