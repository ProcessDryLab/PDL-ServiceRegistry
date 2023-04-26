using System.Text;
using System;
using ServiceRegistry.ConnectedNodes;
using Newtonsoft.Json;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
//using ServiceRegistry.App;

namespace ServiceRegistry.Requests
{
    public class Requests
    {
        static HttpClient client = new HttpClient();
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

        public static async void GetAndSaveConfig(string nodeUrl)
        {
            string requestPath = nodeUrl + "/configurations";
            try
            {
                HttpResponseMessage response = await client.GetAsync(requestPath);
                if (response.IsSuccessStatusCode)
                {
                    string configString = await response.Content.ReadAsStringAsync();
                    JToken config = JToken.Parse(configString);
                    if(config != null) 
                    {
                        ConnectedNodes.ConnectedNodes.Instance.AddConfiguration(nodeUrl, config);
                        //Console.WriteLine($"ConnectedNodes:\nnodeUrl: {nodeUrl}\nconfig{config}");
                    }
                }
            }
            catch
            {
                ConnectedNodes.ConnectedNodes.Instance.AddConfiguration(nodeUrl, "");
            }
        }
    }
}
