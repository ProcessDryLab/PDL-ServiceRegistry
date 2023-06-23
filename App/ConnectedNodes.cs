using Newtonsoft.Json;
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

        public async Task<IResult> AddNode(string bodyString, NodeType type)
        {
            var nodes = GetRegisteredNodes(type);
            bool validRequest = bodyString.TryParseJson(out Dictionary<string, string> bodyDict);
            if (!validRequest || bodyDict == null || bodyDict["Host"] == null) return Results.BadRequest($"Request body: {bodyString} is missing a value for key: \"Host\"");

            string nodeUrl = bodyDict["Host"]; // TODO: Consider deserializing this body to avoid problems with capitalized letters
            
            if (!nodes.Contains(nodeUrl)) // We only add nodes to the file if they don't exist.
            {
                nodes.Add(nodeUrl);
                UpdateNodeFile(nodes, type);
            }
            
            //Writing or overwriting config.
            string configString = await Requests.Requests.GetConfigFromNode(nodeUrl);
            bool configFetched = AddConfiguration(nodeUrl, configString);
            if(!configFetched) return Results.Ok($"Node {nodeUrl} added but unable to get config.");

            return Results.Ok($"Node {nodeUrl} successfully added");
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
            configurations.Remove(nodeUrl);
            onlineStatus.Remove(nodeUrl);

            return Results.Ok($"Node {nodeUrl} successfully removed");
        }

        public void UpdateOnlineStatus()
        {
            GetRegisteredNodes(NodeType.Miner).ForEach(async nodeUrl =>
            {
                onlineStatus[nodeUrl] = await Requests.Requests.GetPing(nodeUrl);
            });

            GetRegisteredNodes(NodeType.Repository).ForEach(async nodeUrl =>
            {
                onlineStatus[nodeUrl] = await Requests.Requests.GetPing(nodeUrl);
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
                string configString = await Requests.Requests.GetConfigFromNode(nodeUrl);
                AddConfiguration(nodeUrl, configString);
            });

            GetRegisteredNodes(NodeType.Repository).ForEach(async nodeUrl =>
            {
                string configString = await Requests.Requests.GetConfigFromNode(nodeUrl);
                AddConfiguration(nodeUrl, configString);
            });
        }

        public Dictionary<string, JToken> GetConfigurations()
        {
            return configurations;
        }

        public bool AddConfiguration(string nodeUrl, string configString)
        {
            if (string.IsNullOrWhiteSpace(nodeUrl) || string.IsNullOrWhiteSpace(configString)) return false;
            JToken config = JToken.Parse(configString);
            configurations[nodeUrl] = config;
            return true;
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
