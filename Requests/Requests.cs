using System.Text;
using System;
using ServiceRegistry.ConnectedNodes;
using Newtonsoft.Json;

namespace ServiceRegistry.Requests
{
    public class Requests
    {
        static HttpClient client = new HttpClient();
        public static async Task<string> GetConfig(string path)
        {
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadAsStringAsync();
                if(res != null) {
                    Node n = JsonConvert.DeserializeObject<Node>(res);
                    n.path = path;
                    ConnectedNodes.ConnectedNodes.Instance.AddNode(path, n);
                }
            }
            return "complete"; //return the result of the call
        }

        public static async Task<bool> GetPing(string path)
        {
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
    }
}
