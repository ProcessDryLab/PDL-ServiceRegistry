using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Xml.Linq;
using ServiceRegistry.Requests;
using Newtonsoft.Json.Linq;
//using ServiceRegistry.App;

namespace ServiceRegistry.ConnectedNodes
{
    public class ConnectedNodes
    {
        private static ConnectedNodes? instance = null;
        static readonly string connectedMinersPath = Path.Combine(Directory.GetCurrentDirectory(), "connectedMiners.json");
        static readonly string connectedRepositoriesPath = Path.Combine(Directory.GetCurrentDirectory(), "connectedRepositories.json");  
        private static Dictionary<string, Boolean> onlineStatus = new();
        private static Dictionary<string, JToken> configurations = new();

        public static ConnectedNodes Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConnectedNodes();
                }
                return instance;
            }
        }

        private static List<string> GetRegisteredNodes(string pathToFile)
        {
            string fileAsString = File.ReadAllText(pathToFile);
            List<string> nodeUrls = JsonConvert.DeserializeObject<List<string>>(fileAsString);
            //Dictionary<string, Node>? nodes = JsonConvert.DeserializeObject<Dictionary<string, Node>>(fileAsString);
            nodeUrls ??= new List<string>();
            return nodeUrls;
        }
        public List<string> GetRegisteredNodes(NodeType type)
        {
            if (type == NodeType.Miner)
            {
                return GetRegisteredNodes(connectedMinersPath);
            }
            if (type == NodeType.Repository)
            {
                return GetRegisteredNodes(connectedRepositoriesPath);
            }
            else return null;
        }
        private static void UpdateNodeFile(List<string> nodeUrls, NodeType type)
        {
            string updatedNodesString = JsonConvert.SerializeObject(nodeUrls, Formatting.Indented);
            if (type == NodeType.Miner)
            {
                File.WriteAllText(connectedMinersPath, updatedNodesString);
            }
            if (type == NodeType.Repository)
            {
                File.WriteAllText(connectedRepositoriesPath, updatedNodesString);
            }
        }

        public IResult AddNode(string bodyString, NodeType type)
        {
            var nodes = GetRegisteredNodes(type);
            bool validRequest = bodyString.TryParseJson(out Dictionary<string, string> bodyDict);
            if (!validRequest || bodyDict == null || bodyDict["Host"] == null) return Results.BadRequest($"Request body: {bodyString} is missing a value for key: \"Host\"");

            string nodeUrl = bodyDict["Host"]; // TODO: Consider deserializing this body to avoid problems with capitalized letters
            nodes.Add(nodeUrl);
            UpdateNodeFile(nodes, type);
            Requests.Requests.GetAndSaveConfig(nodeUrl);

            return Results.Ok($"Repository {nodeUrl} successfully added");
        }
        public IResult RemoveNode(string bodyString, NodeType type)
        {
            var nodes = GetRegisteredNodes(type);
            bool validRequest = bodyString.TryParseJson(out Dictionary<string, string> bodyDict);
            if (!validRequest || bodyDict == null || bodyDict["Host"] == null) return Results.BadRequest($"Request body: {bodyString} is missing a value for key: \"Host\"");

            string nodeUrl = bodyDict["Host"]; // TODO: Consider deserializing this body to avoid problems with capitalized letters
            if (!nodes.Contains(nodeUrl)) return Results.BadRequest($"No node with URL {nodeUrl} exists");
            nodes.Remove(nodeUrl);
            UpdateNodeFile(nodes, type);

            return Results.Ok($"Repository {nodeUrl} successfully removed");
        }

        public void UpdateOnlineStatus()
        {
            GetRegisteredNodes(NodeType.Miner).ForEach(async nodeUrl =>
            {
                onlineStatus[nodeUrl] = await Requests.Requests.GetPing(nodeUrl);
                //Console.WriteLine($"Node with URL {nodeUrl} connection status: {onlineStatus[nodeUrl]}");
            });

            GetRegisteredNodes(NodeType.Repository).ForEach(async nodeUrl =>
            {
                onlineStatus[nodeUrl] = await Requests.Requests.GetPing(nodeUrl);
                //Console.WriteLine($"Node with URL {nodeUrl} connection status: {onlineStatus[nodeUrl]}");
            });
        }

        public Dictionary<string, Boolean> GetOnlineStatus()
        {
            return onlineStatus;
        }

        public void GetAllConnectedHostConfig()
        {

            GetRegisteredNodes(NodeType.Miner).ForEach(async nodeUrl =>
            {
                Requests.Requests.GetAndSaveConfig(nodeUrl);
            });

            GetRegisteredNodes(NodeType.Repository).ForEach(async nodeUrl =>
            {
                Requests.Requests.GetAndSaveConfig(nodeUrl);
            });
        }

        public Dictionary<string, JToken> GetConfigurations()
        {
            return configurations;
        }

        public void AddConfiguration(string key, JToken value)
        {
            if ((key == null || value == null) || (key == "")) return;
            configurations[key] = value;
            //Console.WriteLine($"ConnectedNodes:\nnodeUrl: {key}\nconfig{configurations[key]}");
        }
    }
    #region extensionMethods
    public static class ExtensionClass
    {
        public static string ToStringNullSafe(this object value)
        {
            return (value ?? string.Empty).ToString();
        }

        public static bool TryParseJson<T>(this string obj, out T result)
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.MissingMemberHandling = MissingMemberHandling.Error;

                result = JsonConvert.DeserializeObject<T>(obj, settings);
                return true;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }
    }
    #endregion
}
