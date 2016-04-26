using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Create.CSP.GitHub.Reporting.Helpers
{
    public static class HttpWebRequestExtensionMethods
    {
        /// <summary>
        /// Extension method to encapsulate a try catch web request, reading the response content, and
        /// deserializing it to a dynamic object.
        /// </summary>
        /// <param name="request">The request to make.</param>
        /// <returns>The dynamic deserialized json object.</returns>
        /// <exception cref="WebException">Throws exception with the response content if request fails</exception>
        public static dynamic TryCatchRequest(this HttpWebRequest request)
        {
            try
            {
                // Print request
                //Utilities.PrintWebRequest(request, string.Empty);

                // Make request
                var response = request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    // Read response
                    var responseContent = reader.ReadToEnd();

                    // Print response
                    //Utilities.PrintWebResponse((HttpWebResponse)response, responseContent);
                    return JsonConvert.DeserializeObject(responseContent);
                }
            }
            catch (WebException webException)
            {
                using (var reader = new StreamReader(webException.Response.GetResponseStream()))
                {
                    // Read response
                    var responseContent = reader.ReadToEnd();

                    // Print response error
                    Utilities.PrintErrorResponse((HttpWebResponse)webException.Response, responseContent);
                    dynamic response = JsonConvert.DeserializeObject(responseContent);
                    throw new WebException(responseContent, webException);
                }
            }
        }
    }
}
