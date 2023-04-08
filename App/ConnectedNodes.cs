using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Xml.Linq;

namespace ServiceRegistry.ConnectedNodes
{
    public class ConnectedNodes
    {
        private static ConnectedNodes? instance = null;
        static readonly string connectedMinersPath = Path.Combine(Directory.GetCurrentDirectory(), "connectedMiners.json");
        static readonly string connectedRepositoriesPath = Path.Combine(Directory.GetCurrentDirectory(), "connectedRepositories.json");        
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

        private static Dictionary<string, Node> GetConnectedNodes(string pathToFile)
        {
            string fileAsString = File.ReadAllText(pathToFile);
            Dictionary<string, Node>? nodes = JsonConvert.DeserializeObject<Dictionary<string, Node>>(fileAsString);
            nodes ??= new Dictionary<string, Node>();
            return nodes;
        }
        private static Dictionary<string, Node> GetConnectedNodes(NodeType type)
        {
            if (type == NodeType.Miner)
            {
                return GetConnectedNodes(connectedMinersPath);
            }
            if (type == NodeType.Repository)
            {
                return GetConnectedNodes(connectedRepositoriesPath);
            }
            else return null;
        }
        private static void UpdateConnectedNodes(Dictionary<string, Node> nodes, NodeType type)
        {
            string updatedNodesString = JsonConvert.SerializeObject(nodes, Formatting.Indented);
            if (type == NodeType.Miner)
            {
                File.WriteAllText(connectedMinersPath, updatedNodesString);
            }
            if (type == NodeType.Repository)
            {
                File.WriteAllText(connectedRepositoriesPath, updatedNodesString);
            }
        }

        public void AddNode(string hostName, Node node)
        {
            var nodes = GetConnectedNodes(node.Type);
            nodes.Add(hostName, node);
            UpdateConnectedNodes(nodes, node.Type);
        }
        public void RemoveNode(string hostName, NodeType type)
        {
            var nodes = GetConnectedNodes(type);
            nodes.Remove(hostName);
            UpdateConnectedNodes(nodes, type);
        }
        public Node GetNode(string hostName, NodeType type)
        {
            var nodes = GetConnectedNodes(type);
            return nodes[hostName];
        }
        public List<Node> GetList(NodeType type)
        {
            List<Node> listOfNodes = new();
            Dictionary<string, Node> nodes = GetConnectedNodes(type);
            foreach (var node in nodes)
            {
                node.Value.HostName = node.Key; // Adding host to the contents
                listOfNodes.Add(node.Value);    // Adding updated 
            }
            return listOfNodes;
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
