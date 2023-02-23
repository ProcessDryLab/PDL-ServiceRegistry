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
                // handle something in the response
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
