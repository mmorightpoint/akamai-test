using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Akamai.EdgeGrid.Auth;

namespace AkamaiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = "";
            string clientToken = "";
            string accessToken = "";
            string clientSecret = "";

            try
            {
                // Form the request
                ServicePointManager.Expect100Continue = false;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host + "/ccu/v2/queues/default");
                request.Method = "POST";


                string postData = string.Empty;

                byte[] byteArray = Encoding.Default.GetBytes(postData);
                request.ContentType = "application/json";
                request.ContentLength = byteArray.Length;

                // Sign the request
                using (Stream bodyStream = new MemoryStream(byteArray))
                {
                    EdgeGridV1Signer signer = new EdgeGridV1Signer();
                    ClientCredential credential = new ClientCredential(clientToken, accessToken, clientSecret);
                    signer.Sign(request, credential, bodyStream);
                }

                // Write the payload
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    dataStream.Close();
                }

                // Get the response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                    {
                        Trace.TraceError("Error flushing Akamai cache {0} {1}", response.StatusDescription, postData);
                    }
                    else
                    {
                        Trace.TraceInformation("Successfully flushed " + postData);
                    }

                    response.Close();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }
    }
}
