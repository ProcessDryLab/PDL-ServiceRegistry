using Swashbuckle.AspNetCore.SwaggerGen;

namespace ServiceRegistry.ConnectedNodes
{
    public class ConnectedNodes
    {
        private static ConnectedNodes? instance = null;
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

        private Dictionary<string, Node> nodes = new Dictionary<string, Node>();
        public void AddNode(string path, Node node)
        {
            if(!nodes.ContainsKey(path) && node != null)
            {
                Instance.nodes.Add(path, node);
            }
        }

        public void RemoveNode(string path, Node node)
        {
            nodes.Remove(path);
        }

        public Node GetNode(string path)
        {
            return Instance.nodes[path];
        }

        //public Dictionary<string, Node> Filter(string filterParam)
        public List<Node> Filter(string filterParam)
        {
            //return Instance.nodes.Where(n => n.Value.type == filterParam).ToDictionary(n => n.Key, n => n.Value);

            List<Node> listOfNodes = new List<Node>();

            foreach (KeyValuePair<string, Node> entry in Instance.nodes.Where(n => n.Value.type == filterParam).ToDictionary(n => n.Key, n => n.Value))
            {
                // do something with entry.Value or entry.Key
                listOfNodes.Add(entry.Value);
            }
            return listOfNodes;
        }
    }
}
