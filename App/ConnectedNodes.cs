using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Xml.Linq;
using ServiceRegistry.Requests;
//using ServiceRegistry.App;

namespace ServiceRegistry.ConnectedNodes
{
    public class ConnectedNodes
    {
        private static ConnectedNodes? instance = null;
        static readonly string connectedMinersPath = Path.Combine(Directory.GetCurrentDirectory(), "connectedMiners.json");
        static readonly string connectedRepositoriesPath = Path.Combine(Directory.GetCurrentDirectory(), "connectedRepositories.json");  
        private static Dictionary<string, Boolean> onlineStatus = new();
        private static Dictionary<string, string> configurations = new();

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

        public void AddNode(string nodeUrl, Node node)
        {
            var nodes = GetRegisteredNodes(node.Type);
            nodes.Add(nodeUrl);
            UpdateNodeFile(nodes, node.Type);
        }
        public void RemoveNode(string nodeUrl, NodeType type)
        {
            var nodes = GetRegisteredNodes(type);
            nodes.Remove(nodeUrl);
            UpdateNodeFile(nodes, type);
        }
        //public Node GetNode(string hostName, NodeType type)
        //{
        //    var nodes = GetRegisteredNodes(type);
        //    return nodes[hostName];
        //}
        //public List<Node> GetList(NodeType type)
        //{
        //    List<Node> listOfNodes = new();
        //    Dictionary<string, Node> nodes = GetRegisteredNodes(type);
        //    foreach (var node in nodes)
        //    {
        //        node.Value.HostName = node.Key; // Adding host to the contents
        //        listOfNodes.Add(node.Value);    // Adding updated 
        //    }
        //    return listOfNodes;
        //}

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

        public Dictionary<string, string> GetConfigurations()
        {
            return configurations;
        }

        public void AddConfiguration(string key, string value)
        {
            if ((key == null || value == null) || (key == "" || value == "")) return;
            configurations[key] = value;

            //Console.WriteLine($"ConnectedNodes:\nnodeUrl: {key}\nconfig{configurations[key]}");
        }

        //public List<Node> Filter(string filterParam)
        //{
        //    //return Instance.nodes.Where(n => n.Value.type == filterParam).ToDictionary(n => n.Key, n => n.Value);

        //    List<Node> listOfNodes = new List<Node>();

        //    foreach (KeyValuePair<string, Node> entry in Instance.repositoryNodes.Where(n => n.Value.type == filterParam).ToDictionary(n => n.Key, n => n.Value))
        //    {
        //        // do something with entry.Value or entry.Key
        //        listOfNodes.Add(entry.Value);
        //    }
        //    return listOfNodes;
        //}
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
