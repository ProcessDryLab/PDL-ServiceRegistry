using System.Text;
using System;
using ServiceRegistry.ConnectedNodes;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace ServiceRegistry.Requests
{
    public class Requests
    {
        static HttpClient client = new HttpClient();
        public static async Task<IResult> GetConfig(HttpRequest request)
        {
            // Code if we rather want Form
            //string hostName = request.Form["host"].ToString();
            //Console.WriteLine("hostName: " + hostName);

            // Code if we rather want json body (more safety included here as well)
            var requestBody = new StreamReader(request.Body);
            string requestBodyString = await requestBody.ReadToEndAsync();

            bool requestBodyValid = requestBodyString.TryParseJson(out ListDictionary requestBodyDict);
            if (!requestBodyValid || requestBodyDict == null) return Results.BadRequest("Request body is invalid or empty");

            string? hostName = requestBodyDict["host"].ToStringNullSafe();
            if (string.IsNullOrWhiteSpace(hostName)) return Results.BadRequest("Request body has no host key");

            string requestPath = hostName + "/configurations";
            HttpResponseMessage response = await client.GetAsync(requestPath);
            if (!response.IsSuccessStatusCode) return Results.BadRequest("Bad request");

            var responseString = await response.Content.ReadAsStringAsync();
            if (responseString == null) return Results.BadRequest("Bad request");

            bool validResponseBody = responseString.TryParseJson(out Node node);
            if (!validResponseBody || node == null) return Results.BadRequest("Response body from new node is invalid or empty");

            ConnectedNodes.ConnectedNodes.Instance.AddNode(hostName, node);
            return Results.Accepted($"Node with host name {hostName} added successfully");
        }

        public static async Task<bool> GetPing(string path)
        {
            string requestPath = path + "/ping";
            try
            {
                HttpResponseMessage response = await client.GetAsync(requestPath);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
